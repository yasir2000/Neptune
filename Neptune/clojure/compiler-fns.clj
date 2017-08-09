;;;--------------------
;;; NeptuneScript Compiler
;;;--------------------
;;; Version 2.0 grammar  
;;; October 21, 2011         
;;;--------------------
(ns neptune)
(declare compile-expr)
(declare compile-stmt)
(declare compile-stmts)

;----------------------
(defn compile-boolean [expr]
  (get expr :val))

;----------------------
(defn compile-number [expr]
  (double (get expr :val)))

;----------------------
(defn compile-string [expr]
  (str (get expr :val)))

;----------------------
(defn compile-assign [expr]
  (list
    'var-set
    (get expr :lhs)
    (compile-expr (get expr :rhs))))

;----------------------
(defn compile-identifier [expr]
  (list
    'var-get
    (get expr :symbol)))

;----------------------
(defn compile-op [expr]
  (let*
    [
     op (get expr :op)
     opval (get op :val)
     lhs-expr (get expr :lhs)
     rhs-expr (get expr :rhs)
     ]
    (list
      (symbol opval)
       (compile-expr lhs-expr)
       (compile-expr rhs-expr))))

;----------------------
(defn compile-function [expr]
  (let*
    [
     fn-name (get expr :name)
     fn-args (get expr :args)
     ]
    (list 'apply (symbol fn-name) (vec (map compile-expr fn-args)))))

;----------------------
(defn compile-var [expr]
  (let*
    [
     stmts (compile-stmts (get expr :stmts))
     vars (vec (flatten (for [x (get expr :args)] [x nil])))
     var-expr (list 'with-local-vars vars)
     ]
    (concat var-expr stmts)))

;----------------------
(defn compile-if [expr]
  (let*
    [
     logexp (get expr :logexp)
     true-stmt (get expr :true)
     false-stmt (get expr :false)
     ]
    (list
      'if
      (compile-expr logexp)
      (compile-expr true-stmt)
      (compile-expr false-stmt))))

;----------------------
(defn compile-while [expr]
  (let*
    [
     logexp (get expr :logexp)
     body-stmt (get expr :body)
     ]
    (list
      'loop
      []
      (list
        'if
        (compile-expr logexp)
        (list
          'do
          (compile-expr body-stmt)
          (list 'recur))))))

;----------------------
(defn compile-for [expr]
  (let*
    [
     init-expr (get expr :init-expr)
     limit-expr (get expr :limit-expr)
     update-expr (get expr :update-expr)
     body-stmt (get expr :body-stmt)
     ]
    (list
      'do
      (compile-expr init-expr)
      (list
        'loop
        []
        (list
          'if
          (compile-expr limit-expr)
          (list
            'do
            (compile-expr body-stmt)
            (compile-expr update-expr)
            (list 'recur)))))))

;----------------------
(defn compile-do-while [expr]
  (let*
    [
     logexp (get expr :logexp)
     body-stmt (get expr :body)
     ]
    (list
      'loop
      []
      (compile-expr body-stmt)
      (list
        'if
        (compile-expr logexp)
          (list 'recur)))))

;----------------------
(defn compile-block [expr]
  (let*
    [
     stmts (get expr :stmts)
     ]
    (into '() (reverse (concat ['do] (for [stmt stmts] (compile-stmt stmt)))))))

;----------------------
(defn compile-expr [expr]
  (if (nil? expr)
    nil
    (let*
      [
       type (get expr :type)
       ]
      (case type
        :empty nil
        :block (compile-block expr)
        :var (compile-var expr)
        :if (compile-if expr)
        :while (compile-while expr)
        :for (compile-for expr)
        :do-while (compile-do-while expr)
        :function (compile-function expr)
        :boolean (compile-boolean expr)
        :number (compile-number expr)
        :string (compile-string expr)
        :assign (compile-assign expr)
        :op (compile-op expr)
        :identifier (compile-identifier expr)
        (format "unknown expr type [%s]" type)))))

;----------------------
(defn compile-stmt [stmt]
  (compile-expr stmt)
  )

;----------------------
(defn compile-stmts [stmts]
  (let*
    [
     expr-seq
     (for [stmt stmts] (compile-stmt stmt))
     ]
    (vec expr-seq)))

;----------------------
(defn group-vars [stmts]
  (let*
    [
     var-stmts (filter (fn[x] (= :var (get x :type))) stmts)
     nonvar-stmts (filter (fn[x] (not (= :var (get x :type)))) stmts)
     ]
    (if (empty? var-stmts)
      nonvar-stmts
      (let*
        [
         var-stmt (first var-stmts)
         var-stmt2 (assoc var-stmt :stmts nonvar-stmts)
         ]
        [var-stmt2]))))

;----------------------
(defn compile-actions [actions]
  (let*
    [
     stmts (group-vars actions)
     exprs (compile-stmts stmts)
     ]
    exprs
    ))

;----------------------
(defn compile-task-declaration [task]
  []
  )

;----------------------
(defn compile-process-declaration [process]
  (let*
    [
     args [],
     actions (get process :actions),
     name {:symbol 'start}, ;(get process :name)
     exprs (compile-actions actions),
     fn-expr (into '() (reverse (concat ['fn] [args] exprs)))
     ]
    [(list 'set-method (list 'quote (get name :symbol)) fn-expr)]))

;----------------------
(defn compile-nblo-declaration [nblo]
  (let*
    [
     args (get nblo :args)
     exprs (get nblo :exprs)
     name (get nblo :name)
     fn-expr (into '() (reverse (concat ['fn] [args] exprs)))
     ]
    [(list 'set-method (list 'quote (get name :symbol)) fn-expr)]))

;----------------------
(defn compile-source-element [src-element]
  (case
    (get src-element :type)
    :nblo (compile-nblo-declaration src-element)
    :process (compile-process-declaration src-element)
    :task (compile-task-declaration src-element)
    [])
  )

;----------------------
(defn compile-script [src-code]
  (let*
    [
     source-elements (script src-code)
     compiled-elems (for [elem source-elements] (compile-source-element elem))
     exprs (filter (fn[x](not (empty? x))) compiled-elems)
     ]
    (concat '(fn []) (into '() (reverse exprs)))
    ))

;----------------------
(defn compile-script-fn [script]
  (eval (compile-script script)))

;----------------------
(defn eval-script [script]
  (apply (compile-script-fn script) []))
