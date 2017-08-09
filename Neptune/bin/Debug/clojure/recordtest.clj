(ns clojure.recordtest)

(defrecord R [a b]
  Object
  (GetHashCode [_] (new R (inc a) (inc b)) (prn (+ a b)) (+ a b)))      
    