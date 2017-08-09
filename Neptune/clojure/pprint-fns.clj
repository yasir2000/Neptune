;;;--------------------
;;; Neptune PPrint Functions
;;;--------------------
(declare pp-default)
(declare pp-seq-1)
(declare pp-fn)
(declare pp-if)
(declare pp-list)
(declare pp-n)
(declare pp-pad)
(declare pp-quote)
(declare pp-self)
(declare pp-seq)
(declare pp-seq-nl)
(declare pp-seq-1)
(declare pp-seq-2)
(declare pp-seq-3)
(declare pp-vector)

;----------------------
(defn pprint [expr]
;----------------------
	(pp-n expr 0))

;----------------------
(defn pp-n [expr n]
;----------------------
	(cond
		(list? expr) (pp-list expr n)
		(vector? expr) (pp-vector expr n)
		true (pp-default expr n)))

;----------------------
(defn pp-default [expr n]
;----------------------
	(str
		(pp-pad n)
		(pr-str expr)))

;----------------------
(defn pp-fn [expr n]
;----------------------
	(str
	(pp-pad n)
	"(fn "
	(prn-str (second expr))
	(pp-seq-nl (rest (rest expr)) (+ n 3))
	(pp-pad n)
	")"))

;----------------------
(defn pp-if [expr n]
;----------------------
	(str
	(pp-pad n)
	"(if "
	(prn-str (second expr))
	(pp-seq-nl (rest (rest expr)) (+ n 3))
	(pp-pad n)
	")"))

;----------------------
(defn pp-list [expr n]
;----------------------
	(cond
		(empty? expr)
			(pp-default expr n)
		(not (symbol? (first expr)))
			(pp-default expr n)
		true
			(case (keyword (first expr))
				:do (pp-seq-1 "do" expr n)
				:doseq (pp-seq-2 "doseq" expr n)
				:dotimes (pp-seq-2 "dotimes" expr n)
				:fn (pp-seq-2 "fn" expr n)
				:if (pp-seq-2 "if" expr n)
				:let* (pp-seq-3 "let*" expr n)
				:quote (pp-quote expr n)
				:self (pp-self expr n)
				(pp-default expr n))))

;----------------------
(defn pp-pad [n]
;----------------------
	(.PadLeft "" n))

;----------------------
(defn pp-quote [expr n]
;----------------------
	(str
		(pp-pad n)
		"'"
		(.Trim (pp-n (second expr) n))))

;----------------------
(defn pp-self [expr n]
;----------------------
	(str
		(pp-pad n)
		"(self "
		(pp-seq (rest expr))
		")"))

;----------------------
(defn pp-seq [expr]
;----------------------
	(.Trim (reduce str (for [x expr] (str (pp-n x 0) " ")))))

;----------------------
(defn pp-seq-nl [expr n]
;----------------------
	(reduce str (for [x expr] (println-str (pp-n x n)))))

;----------------------
(defn pp-seq-1 [name expr n]
;----------------------
	(str
	(pp-pad n)
	(println-str (format "(%s " name))
	(pp-seq-nl (rest expr) (+ n 3))
	(pp-pad n)
	")"))

;----------------------
(defn pp-seq-2 [name expr n]
;----------------------
	(str
	(pp-pad n)
	(format "(%s " name)
	(cond
		(list? (second expr))
		(println-str (str "(" (pp-seq (second expr)) ")"))
		(vector? (second expr))
		(println-str (str "[" (pp-seq (second expr)) "]"))
		true (prn-str (second expr)))
	(pp-seq-nl (rest (rest expr)) (+ n 3))
	(pp-pad n)
	")"))

;----------------------
(defn pp-seq-3 [name expr n]
;----------------------
	(str
	(pp-pad n)
	(format "(%s " name)
	(cond
		(list? (second expr))
		(println-str (str "(" (pp-seq-nl (second expr) (+ n 3)) ")"))
		(vector? (second expr))
		(str
			(println-str "")
			(println-str (str (pp-pad (+ n 3)) "["))
			(pp-seq-nl (second expr) (+ n 6))
			(println-str (str (pp-pad (+ n 3)) "]")))
		true (prn-str (second expr)))
	(pp-seq-nl (rest (rest expr)) (+ n 3))
	(pp-pad n)
	")"))
	
;----------------------
(defn pp-vector [expr n]
;----------------------
	(str
	(pp-pad n)
	(println-str "[")
	(pp-seq-nl expr (+ n 3))
	(pp-pad n)
	"]"))
