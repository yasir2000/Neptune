﻿;   Copyright (c) Rich Hickey. All rights reserved.
;   The use and distribution terms for this software are covered by the
;   Eclipse Public License 1.0 (http://opensource.org/licenses/eclipse-1.0.php)
;   which can be found in the file epl-v10.html at the root of this distribution.
;   By using this software in any fashion, you are agreeing to be bound by
;   the terms of this license.
;   You must not remove this notice, or any other, from this software.

(ns clojure.core.protocols)

(defprotocol InternalReduce
  "Protocol for concrete seq types that can reduce themselves
   faster than first/next recursion. Called by clojure.core/reduce."
  (internal-reduce [seq f start]))

(extend-protocol InternalReduce
  nil
  (internal-reduce
   [s f val]
   val)
  
  ;; handles vectors and ranges
  clojure.lang.IChunkedSeq
  (internal-reduce
   [s f val]
   (if-let [s (seq s)]
    (if (chunked-seq? s)
       (recur (chunk-next s)
              f
              (.reduce (chunk-first s) f val))
       (internal-reduce s f val))
	 val))

  clojure.lang.StringSeq
  (internal-reduce
   [str-seq f val]
   (let [s (.S str-seq)]                             ;;; .s
     (loop [i (.I str-seq)                           ;;; .i
            val val]
       (if (< i (.Length s))                         ;;; .length
         (recur (inc i) (f val (.get_Chars s i)))       ;;; .charAt
         val))))
  
  clojure.lang.UntypedArraySeq                             ;;; ArraySeq
  (internal-reduce
       [a-seq f val]
       (let [^objects arr (.Array a-seq)]           ;;; .array
         (loop [i (.Index a-seq)                     ;;; .index
                val val]
           (if (< i (alength arr))
             (recur (inc i) (f val (aget arr i)))
             val))))

  Object                                       ;;;java.lang.Object
  (internal-reduce
   [s f val]
   (loop [cls (class s)
          s s
          f f
          val val]
     (if-let [s (seq s)]
       ;; roll over to faster implementation if underlying seq changes type
       (if (identical? (class s) cls)
         (recur cls (next s) f (f val (first s)))
         (internal-reduce s f val))
       val))))
       
(def arr-impl
  '(internal-reduce
       [a-seq f val]
       (let [arr (.Array a-seq)]                   ;;; .array
         (loop [i (.Index a-seq)                   ;;; .index
                val val]
           (if (< i (alength arr))
             (recur (inc i) (f val (aget arr i)))
             val)))))

(defn- emit-array-impls*
  [syms]
  (apply
   concat
   (map
    (fn [s]
      [(symbol (str "clojure.lang.TypedArraySeq`1[" s "]"))    ;;;  (str "clojure.lang.ArraySeq$ArraySeq_" s)
       arr-impl])
    syms)))
		
(defmacro emit-array-impls
  [& syms]
  `(extend-protocol InternalReduce
     ~@(emit-array-impls* syms)))

;(emit-array-impls int long float double byte char boolean)
(emit-array-impls System.Int32 System.Int64 System.Single System.Double System.Byte System.SByte System.Char System.Boolean 
      System.Int16 System.UInt16 System.UInt32 System.UInt64)

 