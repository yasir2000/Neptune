;;; json.clj: JavaScript Object Notation (JSON) parser/writer

;; by Stuart Sierra, http://stuartsierra.com/
;; January 30, 2010

;; Copyright (c) Stuart Sierra, 2010. All rights reserved.  The use
;; and distribution terms for this software are covered by the Eclipse
;; Public License 1.0 (http://opensource.org/licenses/eclipse-1.0.php)
;; which can be found in the file epl-v10.html at the root of this
;; distribution.  By using this software in any fashion, you are
;; agreeing to be bound by the terms of this license.  You must not
;; remove this notice, or any other, from this software.

(ns ^{:author "Stuart Sierra"
       :doc "JavaScript Object Notation (JSON) parser/writer.
  See http://www.json.org/
  To write JSON, use json-str, write-json, or write-json.
  To read JSON, use read-json."}
    clojure.data.json
  (:use [clojure.pprint :only (write formatter-out)])
  (:import  (System.IO TextWriter TextReader EndOfStreamException StringReader StringWriter Stream StreamWriter)     ;;;(java.io PrintWriter PushbackReader StringWriter
            (System.Text StringBuilder)            
			(clojure.lang PushbackTextReader)))                                       ;;;    StringReader Reader EOFException)))

;;; JSON READER

(declare read-json-reader)

(defn- read-json-array [^PushbackTextReader stream keywordize?]                                 ;;; ^PushbackReader
  ;; Expects to be called with the head of the stream AFTER the
  ;; opening bracket.
  (loop [i (.Read stream), result (transient [])]                                               ;;;   .read
    (when (neg? i) (throw (EndOfStreamException. "JSON error (end-of-file inside array)")))     ;;; EOFException
    (let [c (char i)]
      (cond
       (Char/IsWhiteSpace c) (recur (.Read stream) result)                           ;;; Character/isWhitespace .read
       (= c \,) (recur (.Read stream) result)                                        ;;; .read
       (= c \]) (persistent! result)
       :else (do (.Unread stream (int c))                                            ;;; .unread
                 (let [element (read-json-reader stream keywordize? true nil)]
                   (recur (.Read stream) (conj! result element))))))))               ;;; .read

(defn- read-json-object [^PushbackTextReader stream keywordize?]                     ;;; ^PushbackReader
  ;; Expects to be called with the head of the stream AFTER the
  ;; opening bracket.
  (loop [i (.Read stream), key nil, result (transient {})]                                       ;;; .read
    (when (neg? i) (throw (EndOfStreamException. "JSON error (end-of-file inside array)")))      ;;; EOFException
    (let [c (char i)]
      (cond
       (Char/IsWhiteSpace c) (recur (.Read stream) key result)                       ;;; Character/isWhitespace .read

       (= c \,) (recur (.Read stream) nil result)                                    ;;; .read

       (= c \:) (recur (.Read stream) key result)                                    ;;; .read

       (= c \}) (if (nil? key)
                  (persistent! result)
                  (throw (Exception. "JSON error (key missing value in object)")))

       :else (do (.Unread stream i)                                                  ;;; .unread
                 (let [element (read-json-reader stream keywordize? true nil)]
                   (if (nil? key)
                     (if (string? element)
                       (recur (.Read stream) element result)                         ;;; .read 
                       (throw (Exception. "JSON error (non-string key in object)")))
                     (recur (.Read stream) nil                                       ;;; .read 
                            (assoc! result (if keywordize? (keyword key) key)
                                    element)))))))))

(defn- read-json-hex-character [^PushbackTextReader stream]                          ;;; ^PushbackReader
  ;; Expects to be called with the head of the stream AFTER the
  ;; initial "\u".  Reads the next four characters from the stream.
  (let [digits [(.Read stream)                                    ;;; .read 
                (.Read stream)                                    ;;; .read 
                (.Read stream)                                    ;;; .read 
                (.Read stream)]]                                  ;;; .read 
    (when (some neg? digits)
      (throw (EndOfStreamException. "JSON error (end-of-file inside Unicode character escape)")))      ;;; EOFException
    (let [chars (map char digits)]
      (when-not (every? #{\0 \1 \2 \3 \4 \5 \6 \7 \8 \9 \a \b \c \d \e \f \A \B \C \D \E \F}
                        chars)
        (throw (Exception. "JSON error (invalid hex character in Unicode character escape)")))
      (char (Int32/Parse (apply str chars) System.Globalization.NumberStyles/HexNumber)))))            ;;; Integer/parseInt

(defn- read-json-escaped-character [^PushbackTextReader stream]                                        ;;; ^PushbackReader
  ;; Expects to be called with the head of the stream AFTER the
  ;; initial backslash.
  (let [c (char (.Read stream))]                                                                       ;;; .read 
    (cond
     (#{\" \\ \/} c) c
     (= c \b) \backspace
     (= c \f) \formfeed
     (= c \n) \newline
     (= c \r) \return
     (= c \t) \tab
     (= c \u) (read-json-hex-character stream))))

(defn- read-json-quoted-string [^PushbackTextReader stream]                                        ;;; ^PushbackReader
  ;; Expects to be called with the head of the stream AFTER the
  ;; opening quotation mark.
  (let [buffer (StringBuilder.)]
    (loop [i (.Read stream)]                                                                       ;;; .read 
      (when (neg? i) (throw (EndOfStreamException. "JSON error (end-of-file inside array)")))      ;;; EOFException
      (let [c (char i)]
        (cond
         (= c \") (str buffer)
         (= c \\) (do (.Append buffer (read-json-escaped-character stream))          ;;; .append
                      (recur (.Read stream)))                                        ;;; .read 
         :else (do (.Append buffer c)                                                ;;; .append
                   (recur (.Read stream))))))))                                      ;;; .read 

(defn- read-json-reader
  ([^PushbackTextReader stream keywordize? eof-error? eof-value]                     ;;; ^PushbackReader stream
     (loop [i (.Read stream)]                                                        ;;; .read 
       (if (neg? i) ;; Handle end-of-stream
	 (if eof-error?
	   (throw (EndOfStreamException. "JSON error (end-of-file)"))                    ;;; EOFException
	   eof-value)
	 (let [c (char i)]
	   (cond
	    ;; Ignore whitespace
	    (Char/IsWhiteSpace c) (recur (.Read stream))                                  ;;; Character/isWhitespace  .read

	    ;; Read numbers, true, and false with Clojure reader
	    (#{\- \0 \1 \2 \3 \4 \5 \6 \7 \8 \9} c)
	    (do (.Unread stream i)                                                        ;;; .unread
		(read stream true nil))

	    ;; Read strings
	    (= c \") (read-json-quoted-string stream)

	    ;; Read null as nil
	    (= c \n) (let [ull [(char (.Read stream))                  ;;; .read 
				(char (.Read stream))                              ;;; .read 
				(char (.Read stream))]]                            ;;; .read 
		       (if (= ull [\u \l \l])
			 nil
			 (throw (Exception. (str "JSON error (expected null): " c ull)))))

	    ;; Read true
	    (= c \t) (let [rue [(char (.Read stream))                  ;;; .read 
				(char (.Read stream))                              ;;; .read 
				(char (.Read stream))]]                            ;;; .read 
		       (if (= rue [\r \u \e])
			 true
			 (throw (Exception. (str "JSON error (expected true): " c rue)))))

	    ;; Read false
	    (= c \f) (let [alse [(char (.Read stream))                  ;;; .read 
				 (char (.Read stream))                              ;;; .read 
				 (char (.Read stream))                              ;;; .read 
				 (char (.Read stream))]]                            ;;; .read 
		       (if (= alse [\a \l \s \e])
			 false
			 (throw (Exception. (str "JSON error (expected false): " c alse)))))

	    ;; Read JSON objects
	    (= c \{) (read-json-object stream keywordize?)

	    ;; Read JSON arrays
	    (= c \[) (read-json-array stream keywordize?)

	    :else (throw (Exception. (str "JSON error (unexpected character): " c)))))))))

(defprotocol Read-JSON-From
  (read-json-from [input keywordize? eof-error? eof-value]
                  "Reads one JSON value from input String or Reader.
  If keywordize? is true, object keys will be converted to keywords.
  If eof-error? is true, empty input will throw an EOFException; if
  false EOF will return eof-value. "))

(extend-protocol
 Read-JSON-From
 String
 (read-json-from [input keywordize? eof-error? eof-value]
                 (read-json-reader (PushbackTextReader. (StringReader. input))        ;;; PushbackReader
                                   keywordize? eof-error? eof-value))
 PushbackTextReader                                                                   ;;; PushbackReader
 (read-json-from [input keywordize? eof-error? eof-value]
                 (read-json-reader input
                                   keywordize? eof-error? eof-value))
 TextReader                                                                               ;;; DM: added
 (read-json-from [input keywordize? eof-error? eof-value]
                 (read-json-reader (PushbackTextReader. input)                        
                                   keywordize? eof-error? eof-value))
 Stream                                                                               ;;; Reader
 (read-json-from [input keywordize? eof-error? eof-value]
                 (read-json-reader (PushbackTextReader. input)                        ;;; PushbackReader
                                   keywordize? eof-error? eof-value)))

(defn read-json
  "Reads one JSON value from input String or Reader.
  If keywordize? is true (default), object keys will be converted to
  keywords.  If eof-error? is true (default), empty input will throw
  an EOFException; if false EOF will return eof-value. "
  ([input]
     (read-json-from input true true nil))
  ([input keywordize?]
     (read-json-from input keywordize? true nil))
  ([input keywordize? eof-error? eof-value]
     (read-json-from input keywordize? eof-error? eof-value)))


;;; JSON PRINTER

(defprotocol Write-JSON
  (write-json [object out escape-unicode?]
              "Print object to PrintWriter out as JSON"))

(defn- write-json-string [^String s ^TextWriter out escape-unicode?]           ;;; ^CharSequence  ^PrintWriter
  (let [sb (StringBuilder. ^Int32 (count s))                                        ;;; ^Integer 
         chars32 (System.Globalization.StringInfo/ParseCombiningCharacters s)]                         ;;; DM: added
    (.Append sb \")                                                            ;;; .append
    (dotimes [i (count chars32)]                                               ;;; (count s)
      (let [cp (Char/ConvertToUtf32 s (aget chars32 i)) ]                     ;;; (int (Character/codePointAt s i))]
        (cond
         ;; Handle printable JSON escapes before ASCII
         (= cp 34) (.Append sb "\\\"")                    ;;; .append
         (= cp 92) (.Append sb "\\\\")                    ;;; .append
         (= cp 47) (.Append sb "\\/")                     ;;; .append
         ;; Print simple ASCII characters
         (< 31 cp 127) (.Append sb (.get_Chars s i))          ;;; .append   .charAt
         ;; Handle non-printable JSON escapes
         (= cp 8) (.Append sb "\\b")        ;;; .append
         (= cp 12) (.Append sb "\\f")       ;;; .append
         (= cp 10) (.Append sb "\\n")       ;;; .append
         (= cp 13) (.Append sb "\\r")       ;;; .append
         (= cp 9) (.Append sb "\\t")        ;;; .append
	 ;; Any other character is Unicode
         :else (if escape-unicode?
		 ;; Hexadecimal-escaped
		 (.Append sb (format "\\u%04x" cp))        ;;; .append
		 (.Append sb  (Char/ConvertFromUtf32 cp))))))           ;;; appendCodePoint
    (.Append sb \")                                ;;; .append
    (.Write out (str sb))))                        ;;; .print

(defn- as-str
  [x]
  (if (instance? clojure.lang.Named x)
    (name x)
    (str x)))

(defn- write-json-object [m ^TextWriter out escape-unicode?]          ;;; ^PrintWriter 
  (.Write out \{)
  (loop [x m]
    (when (seq m)
      (let [[k v] (first x)]
        (when (nil? k)
          (throw (Exception. "JSON object keys cannot be nil/null")))
	(write-json-string (as-str k) out escape-unicode?)
        (.Write out \:)                                               ;;; .print
        (write-json v out escape-unicode?))
      (let [nxt (next x)]
        (when (seq nxt)
          (.Write out \,)                                             ;;; .print
          (recur nxt)))))
  (.Write out \}))                                                    ;;; .print

(defn- write-json-array [s ^TextWriter out escape-unicode?]           ;;; ^PrintWriter 
  (.Write out \[)                                                     ;;; .print
  (loop [x s]
    (when (seq x)
      (let [fst (first x)
            nxt (next x)]
        (write-json fst out escape-unicode?)
        (when (seq nxt)
          (.Write out \,)                                             ;;; .print
          (recur nxt)))))
  (.Write out \]))                                                    ;;; .print

(defn- write-json-bignum [x ^TextWriter out escape-unicode]           ;;; ^PrintWriter 
  (.Write out (str x)))                                               ;;; .print

(defn- write-json-plain [x ^TextWriter out escape-unicode?]           ;;; ^PrintWriter 
  (.Write out x))                                                     ;;; .print

(defn- write-json-null [x ^TextWriter out escape-unicode?]            ;;; ^PrintWriter 
  (.Write out "null"))                                                ;;; .print

(defn- write-json-named [x ^TextWriter out escape-unicode?]           ;;; ^PrintWriter 
  (write-json-string (name x) out escape-unicode?))

(defn- write-json-generic [x out escape-unicode?]
  (if (.IsArray (class x))                                            ;;; .isArray
    (write-json (seq x) out escape-unicode?)
    (throw (Exception. (str "Don't know how to write JSON of " (class x))))))

(defn- write-json-ratio [x out escape-unicode?]
  (write-json (double x) out escape-unicode?))

;(extend nil Write-JSON
;        {:write-json write-json-null})
;(extend clojure.lang.Named Write-JSON
;        {:write-json write-json-named})
;(extend java.lang.Boolean Write-JSON
;        {:write-json write-json-plain})
;(extend java.lang.Number Write-JSON
;        {:write-json write-json-plain})
;(extend java.math.BigInteger Write-JSON
;        {:write-json write-json-bignum})
;(extend java.math.BigDecimal Write-JSON
;        {:write-json write-json-bignum})
;(extend clojure.lang.Ratio Write-JSON
;        {:write-json write-json-ratio})
;(extend java.lang.CharSequence Write-JSON
;        {:write-json write-json-string})
;(extend java.util.Map Write-JSON
;        {:write-json write-json-object})
;(extend java.util.Collection Write-JSON
;        {:write-json write-json-array})
;(extend clojure.lang.ISeq Write-JSON
;        {:write-json write-json-array})
;(extend java.lang.Object Write-JSON
;        {:write-json write-json-generic})

(extend nil Write-JSON
        {:write-json write-json-null})
(extend clojure.lang.Named Write-JSON
        {:write-json write-json-named})
(extend System.Boolean Write-JSON
        {:write-json write-json-plain})
;;; no equivalent to java.lang.Number.  Sigh.
(extend System.Byte Write-JSON {:write-json write-json-plain})
(extend System.SByte Write-JSON {:write-json write-json-plain})
(extend System.Int16 Write-JSON {:write-json write-json-plain})
(extend System.Int32 Write-JSON {:write-json write-json-plain})
(extend System.Int64 Write-JSON {:write-json write-json-plain})
(extend System.UInt16 Write-JSON {:write-json write-json-plain})
(extend System.UInt32 Write-JSON {:write-json write-json-plain})
(extend System.UInt64 Write-JSON {:write-json write-json-plain})
(extend System.Double Write-JSON {:write-json write-json-plain})
(extend System.Single Write-JSON {:write-json write-json-plain})
(extend System.Decimal Write-JSON {:write-json write-json-plain})
(extend clojure.lang.BigInt Write-JSON
        {:write-json write-json-bignum})
(extend clojure.lang.BigInteger Write-JSON
        {:write-json write-json-bignum})
(extend clojure.lang.BigDecimal Write-JSON
        {:write-json write-json-bignum})
(extend clojure.lang.Ratio Write-JSON
        {:write-json write-json-ratio})
(extend System.String Write-JSON
        {:write-json write-json-string})
(extend clojure.lang.IPersistentMap Write-JSON
        {:write-json write-json-object})
(extend System.Collections.IDictionary Write-JSON
        {:write-json write-json-object})
;;; Cannot handle generic types!!!!
(extend System.Collections.ICollection Write-JSON
        {:write-json write-json-array})
(extend clojure.lang.ISeq Write-JSON
        {:write-json write-json-array})
(extend System.Object Write-JSON
        {:write-json write-json-generic})
		
(defn json-str
  "Converts x to a JSON-formatted string.

  Valid options are:
    :escape-unicode false
        to turn of \\uXXXX escapes of Unicode characters."
  [x & options]
  (let [{:keys [escape-unicode] :or {escape-unicode true}} options
	sw (StringWriter.)
        ] ;;;out (StreamWriter. sw)]                                        ;;; PrintWriter.
    (write-json x sw escape-unicode)                                   ;;; out => sw
    (.ToString sw)))                                                 ;;; .toString

(defn print-json
  "Write JSON-formatted output to *out*.

  Valid options are:
    :escape-unicode false
        to turn off \\uXXXX escapes of Unicode characters."
  [x & options]
  (let [{:keys [escape-unicode] :or {escape-unicode true}} options]
    (write-json x *out* escape-unicode)))


;;; JSON PRETTY-PRINTER

;; Based on code by Tom Faulhaber

(defn- pprint-json-array [s escape-unicode] 
  ((formatter-out "~<[~;~@{~w~^, ~:_~}~;]~:>") s))

(defn- pprint-json-object [m escape-unicode]
  ((formatter-out "~<{~;~@{~<~w:~_~w~:>~^, ~_~}~;}~:>") 
   (for [[k v] m] [(as-str k) v])))

(defn- pprint-json-generic [x escape-unicode]
  (if (.IsArray (class x))                                                ;;; .isArray
    (pprint-json-array (seq x) escape-unicode)
    (print (json-str x :escape-unicode escape-unicode))))
  
(defn- pprint-json-dispatch [x escape-unicode]
  (cond (nil? x) (print "null")
        (true? x) (print "true")                    ;;; DM: Added -- otherwise we print 'True'
		(false? x) (print "false")                  ;;; DM: Added -- otherwise we print 'False'
        (instance? System.Collections.IDictionary x) (pprint-json-object x escape-unicode)             ;;; java.util.Map
        (instance? System.Collections.ICollection x) (pprint-json-array x escape-unicode)
        (instance? clojure.lang.ISeq x) (pprint-json-array x escape-unicode)
        :else (pprint-json-generic x escape-unicode)))

(defn pprint-json
  "Pretty-prints JSON representation of x to *out*.

  Valid options are:
    :escape-unicode false
        to turn off \\uXXXX escapes of Unicode characters."
  [x & options]
  (let [{:keys [escape-unicode] :or {escape-unicode true}} options]
    (write x :dispatch #(pprint-json-dispatch % escape-unicode))))
