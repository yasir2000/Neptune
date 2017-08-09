;;;--------------------
;;; NeptuneScript Tokenizer
;;;--------------------
;;; Version 2.0 grammar  
;;; October 21, 2011         
;;;--------------------
(ns neptune)

;----------------------
; Characters
;----------------------
(def QUOTE "\"")

;----------------------
; Booleans
;----------------------
(def FALSE "false")
(def TRUE "true")

;----------------------
(def BOOLEANS 
[
 FALSE, TRUE
 ])

;----------------------
; Keywords
;----------------------
(def ACTIONS "actions")
(def DO "do")
(def ELSE "else")
(def FOR "for")
(def IF "if")
(def NBLO "nblo")
(def PROCESS "process")
(def TASK "task")
(def VAR "var")
(def VIA "via")
(def WHILE "while")

;----------------------
(def KEYWORDS 
[
 ACTIONS,
 DO,
 ELSE,
 FOR,
 IF,
 NBLO,
 PROCESS,
 TASK,
 VAR,
 VIA,
 WHILE
 ])

;----------------------
; Punctuators
;----------------------
(def COLON ":")
(def COMMA ",")
(def EQ "=")
(def LBRACE "{")
(def LPAREN "(")
(def RBRACE "}")
(def RPAREN ")")
(def SEMI ";")

;----------------------
(def PUNCTUATORS 
[
 COLON, COMMA, EQ, LBRACE, LPAREN, RBRACE, RPAREN, SEMI
 ])

;----------------------
; Other
;----------------------
(def LNATIVE "{!-")
(def RNATIVE "-!}")

;----------------------
; Multiplicative Ops
;----------------------
(def ASTERISK "*")
(def DIVIDE "/")
(def PERCENT "%")

;----------------------
(def MULTIPLICATIVE-OPS 
[
 ASTERISK, DIVIDE, PERCENT
 ])

;----------------------
; Additive Ops
;----------------------
(def PLUS "+")
(def MINUS "-")

;----------------------
(def ADDITIVE-OPS 
[
 PLUS, MINUS
 ])

;----------------------
; Relational Ops
;----------------------
(def LT "<")
(def GT ">")

;----------------------
(def RELATIONAL-OPS 
[
 LT, GT
 ])

;----------------------
; Logical Ops
;----------------------
(def AMPER "&")
(def BAR "|")

;----------------------
(def LOGICAL-OPS 
[
 AMPER, BAR
 ])

;----------------------
; Replacement Ops
;----------------------
(def AND "and")
(def OR "or")
(def MOD "mod")

;----------------------
(defn error-token [s]
  (let*
    [
     val @s
     ]
    (var-set s "")
    {:type :error, :val val}))

;----------------------
(defn multop [s]
  (let*
    [
     val (.Substring @s 0 1)
     ]
    (var-set s (.Substring @s 1))
    (if (= val PERCENT)
      {:type :multop, :val MOD}
      {:type :multop, :val val})))

;----------------------
(defn addop [s]
  (let*
    [
     val (.Substring @s 0 1)
     ]
    (var-set s (.Substring @s 1))
    {:type :addop, :val val}))

;----------------------
(defn relop [s]
  (let*
    [
     val (.Substring @s 0 1)
     ]
    (var-set s (.Substring @s 1))
    (if (= (first @s) \=)
      (do
        (var-set s (.Substring @s 1))
        {:type :relop, :val (str val "=")})
      {:type :relop, :val val})))

;----------------------
(defn logop [s]
  (let*
    [
     val (.Substring @s 0 1)
     ]
    (var-set s (.Substring @s 1))
    (cond
      (and (= val BAR) (= (.Substring @s 0 1) BAR))
      (do
        (var-set s (.Substring @s 1))
        {:type :logor, :val OR})
      (and (= val AMPER) (= (first @s) \&))
      (do
        (var-set s (.Substring @s 1))
        {:type :logand, :val AND})
      true
      {:type :logand, :val val})))

;----------------------
(defn punctuator [s]
  (let*
    [
     val (.Substring @s 0 1)
     ]
    (var-set s (.Substring @s 1))
    (if (and (= val "=") (= (first @s) \=))
      (do
        (var-set s (.Substring @s 1))
        {:type :eqop, :val (str val "=")})
      {:type :punctuator, :val val})))

;----------------------
(defn number-literal [s]
  (let*
    [n (count @s)]
    (with-local-vars [i 0]
      (loop []
        (if (and
              (< @i n)
              (Char/IsDigit @s @i))
          (do
            (var-set i (inc @i))
            (recur))))
      (let*
        [
         val (.Substring @s 0 @i)
         ]
        (var-set s (.Substring @s @i))
        {
         :type :number,
         :val val
         }
        ))))

 ;----------------------
(defn string-literal [s]
  (let*
    [n (count @s)]
    (with-local-vars [i 1]
      (loop []
        (let*
          [ch (get @s @i)]
          (if (and
                (< @i n)
                (not (= ch (first QUOTE))))
            (do
              (var-set i (inc @i))
              (recur))
            (if (= ch (first QUOTE))
              (var-set i (inc @i))))))
      (let*
        [
         val (.Substring @s 1 (- @i 2))
         ]
        (var-set s (.Substring @s @i))
        {
         :type :string,
         :val val
         }
        ))))

;----------------------
(defn boolean-token [sym]
  {
   :type :boolean,
   :val (= sym TRUE)
   })

;----------------------
(defn keyword-token [sym]
  {
   :type :keyword,
   :val sym
   })

;----------------------
(defn identifier-token [sym]
  {
   :type :identifier,
   :val sym
   })

;----------------------
(defn symbol-token [s]
  (let*
    [n (count @s)]
    (with-local-vars [i 0]
      (loop []
        (if (and
              (< @i n)
              (Char/IsLetterOrDigit @s @i))
          (do
            (var-set i (inc @i))
            (recur))))
      (let*
        [
         sym (.Substring @s 0 @i)
         ]
        (var-set s (.Substring @s @i))
        (cond
          (is_member sym BOOLEANS)
          (boolean-token sym)
          (is_member sym KEYWORDS)
          (keyword-token sym)
          true
          (identifier-token sym))))))

;----------------------
(defn single-line-comment [s]
  (let*
    [
     i (.IndexOf @s \newline)
     s2 @s
     ]
    (if (< i 0)
      (var-set s "")
      (var-set s (.Substring @s i)))
    {
     :type :comment,
     :val (if (< i 0) s2 (.Trim (.Substring s2 2 (- i 2))))
     }))

;----------------------
(defn native-code [s]
  (let*
    [
     rindex (.IndexOf @s RNATIVE)
     s2 @s
     ]
    (if (< rindex 0)
      (do
        (var-set s "")
        {
         :type :error,
         :val s2
         })
      (let*
        [
         code (.Substring @s 3 (- rindex 3))
         ]
        (var-set s (.Substring @s (+ rindex 3)))
        {
         :type :native,
         :val code
         }))))

;----------------------
(defn read-token [s]
  (let*
    [
     ch (first @s)
     ]
    (cond
      (and
        (= ch (first DIVIDE))
        (= (second @s) (first DIVIDE)))
      (single-line-comment s)
      (and
        (= ch (first LNATIVE))
        (= (second @s) (second LNATIVE))
        (= (second (rest @s)) (second (rest LNATIVE))))
      (native-code s)
      (Char/IsDigit ch)
      (number-literal s)
      (= ch (first QUOTE))
      (string-literal s)
      (Char/IsLetter ch)
      (symbol-token s)
      (is_member (str ch) PUNCTUATORS)
      (punctuator s)
      (is_member (str ch) MULTIPLICATIVE-OPS)
      (multop s)
      (is_member (str ch) ADDITIVE-OPS)
      (addop s)
      (is_member (str ch) RELATIONAL-OPS)
      (relop s)
      (is_member (str ch) LOGICAL-OPS)
      (logop s)
      true
      (error-token s))))

;----------------------
(defn remove-leading-whitespace [s]
  (.TrimStart s nil))

;----------------------
(defn tokenize [src]
  (with-local-vars
    [
     s src
     tokens []
     ]
    (loop []
      (var-set s (remove-leading-whitespace @s))
      (if (zero? (count @s))
        @tokens
        (let*
          [
           token (read-token s)
           ]
          (if (not (= (get token :type) :comment))
            (var-set tokens (conj @tokens token)))
          (recur))))
    @tokens))