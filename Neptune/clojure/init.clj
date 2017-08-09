;;;--------------------
;;; Neptune IDE Init
;;;--------------------

;----------------------
(defn load-lib [name]
	(load-file (format "../../clojure/%s.clj" name)))

;----------------------
; Base Libraries
;----------------------
(load-lib 'core-fns)
(load-lib 'pprint-fns)
(load-lib 'tokenizer-fns)
(load-lib 'parser-fns)
(load-lib 'compiler-fns)
(load-lib 'script-fns)
(load-lib 'goal-fns)
(load-lib 'method-fns)
(load-lib 'nblo-fns)
(load-lib 'ide-fns)
