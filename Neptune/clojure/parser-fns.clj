;;;--------------------
;;; NeptuneScript Parser
;;;--------------------
;;; Version 2.0 grammar  
;;; October 21, 2011         
;;;--------------------
(ns neptune)
(declare block)
(declare expression)
(declare identifier)
(declare statement)
(declare primary-expression)

;----------------------
(defn advance [tokens]
  (var-set tokens (rest @tokens)))

;----------------------
(defn peek-token [tokens]
  (if (zero? (count @tokens))
    nil
    (first @tokens)))

;----------------------
(defn peek-token2 [tokens]
  (if (< (count @tokens) 2)
    nil
    (second @tokens)))

;----------------------
(defn next-token [tokens]
  (let*
    [token (peek-token tokens)]
    (advance tokens)
    token))

;----------------------
(defn is-eof [tokens]
  (zero? (count @tokens)))

;----------------------
(defn is-multop [token]
    (= (get token :type) :multop))

;----------------------
(defn is-addop [token]
    (= (get token :type) :addop))

;----------------------
(defn is-relop [token]
    (= (get token :type) :relop))

;----------------------
(defn is-logand [token]
    (= (get token :type) :logand))

;----------------------
(defn is-logor [token]
    (= (get token :type) :logor))

;----------------------
(defn is-eqop [token]
    (= (get token :type) :eqop))

;----------------------
(defn is-punctuator [token value]
  (and 
    (= (get token :type) :punctuator)
    (= (get token :val) value)))

;----------------------
(defn consume-punctuator-if [tokens value]
  (if (is-punctuator (first @tokens) value)
    (do
      (advance tokens)
      true)
    false))

;----------------------
(defn consume-punctuator [tokens value]
  (if (not (consume-punctuator-if tokens value))
    (do
      (println "Token not found: " value @tokens)
      (var-set tokens []))))

;----------------------
(defn consume-optional-comma [tokens]
  (consume-punctuator-if tokens COMMA))

;----------------------
(defn is-assignment [token]
  (is-punctuator token EQ))

;----------------------
(defn is-end-of-stmt [token]
  (is-punctuator token SEMI))

;----------------------
(defn is-left-brace [token]
  (is-punctuator token LBRACE))

;----------------------
(defn is-left-paren [token]
  (is-punctuator token LPAREN))

;----------------------
(defn is-right-brace [token]
  (is-punctuator token RBRACE))

;----------------------
(defn is-right-paren [token]
  (is-punctuator token RPAREN))

;----------------------
(defn is-literal [token]
  (or
    (= (get token :type) :boolean)
    (= (get token :type) :number)
    (= (get token :type) :string)))

;----------------------
(defn is-identifier [token]
  (= (get token :type) :identifier))

;----------------------
(defn is-boolean [token]
  (= (get token :type) :boolean))

;----------------------
(defn is-keyword [token]
  (= (get token :type) :keyword))

;----------------------
(defn is-keyword-do [token]
  (and
    (is-keyword token)
    (= (get token :val) DO)))

;----------------------
(defn is-keyword-else [token]
  (and
    (is-keyword token)
    (= (get token :val) ELSE)))

;----------------------
(defn is-keyword-for [token]
  (and
    (is-keyword token)
    (= (get token :val) FOR)))

;----------------------
(defn is-keyword-if [token]
  (and
    (is-keyword token)
    (= (get token :val) IF)))

;----------------------
(defn is-keyword-nblo [token]
  (and
    (is-keyword token)
    (= (get token :val) NBLO)))

;----------------------
(defn is-keyword-process [token]
  (and
    (is-keyword token)
    (= (get token :val) PROCESS)))

;----------------------
(defn is-keyword-task [token]
  (and
    (is-keyword token)
    (= (get token :val) TASK)))

;----------------------
(defn is-keyword-while [token]
  (and
    (is-keyword token)
    (= (get token :val) WHILE)))

;----------------------
(defn is-native [token]
  (= (get token :type) :native))

;----------------------
(defn assignment-expr [sym tokens]
  (advance tokens)
  {:type :assign, :lhs (symbol (get sym :val)), :rhs (expression tokens)}
  )

;----------------------
(defn literal-expr [tokens]
  (let*
    [
     token (next-token tokens)
     ]
    token
  ))

;----------------------
(defn formal-args [tokens]
  (with-local-vars
    [args []]
    (consume-punctuator tokens LPAREN)
    (loop []
      (if (and
            (not (is-eof tokens))
            (not (is-right-paren (first @tokens))))
        (do
          (var-set args (conj @args (identifier tokens)))
          (consume-optional-comma tokens)
          (recur))))
    (consume-punctuator tokens RPAREN)
    (vec (for [arg @args] (get arg :symbol)))
  ))

;----------------------
(defn function-expr [name tokens]
  (with-local-vars
    [args []]
    (advance tokens)
    (loop []
      (if (and
            (not (is-eof tokens))
            (not (is-right-paren (first @tokens))))
        (do
          (var-set args (conj @args (expression tokens)))
          (consume-optional-comma tokens)
          (recur))))
    (consume-punctuator tokens RPAREN)
    {:type :function :name name, :args @args}
  ))

;----------------------
(defn variable-declaration-list [tokens]
  (with-local-vars
    [args []]
    (loop []
      (if (and
            (not (is-eof tokens))
            (not (is-end-of-stmt (first @tokens))))
        (do
          (let*
            [
             token (next-token tokens)
             ]
            (if (is-identifier token)
              (var-set args (conj @args (symbol (get token :val)))))
            (consume-optional-comma tokens)
            (recur)))))
    @args))

;----------------------
(defn error-expr [name tokens]
  {:type :error :name name, :args @tokens})

;----------------------
(defn identifier [tokens]
  {:type :identifier, :symbol (symbol (get (next-token tokens) :val))}
  )

;----------------------
(defn paren-expression [tokens]
  (advance tokens)
  (let*
    [
     expr (expression tokens)
     ]
    (consume-punctuator tokens RPAREN)
    expr))

;----------------------
(defn primary-expression [tokens]
  (let*
    [
     token (peek-token tokens)
     ]
    (cond
      (is-identifier token)
      (identifier tokens)
      (is-literal token)
      (literal-expr tokens)
      (is-left-paren token)
      (paren-expression tokens)
      true
      nil)))

;----------------------
(defn member-expression [tokens]
  (primary-expression tokens))

;----------------------
(defn call-expression [tokens]
  (let*
    [
     expr
     (if (is-left-paren (peek-token2 tokens))
       (function-expr (get (next-token tokens) :val) tokens)
       (member-expression tokens))
     ]
    expr))

;----------------------
(defn unary-expression [tokens]
  (let*
    [
     expr
     (call-expression tokens)
     ]
    expr))

;----------------------
(defn multiplicative-tail [lhs tokens]
  (let*
    [
     op (next-token tokens)
     rhs (unary-expression tokens)
     ]
    (if (is-multop (peek-token tokens))
      (multiplicative-tail {:type :op :op op, :lhs lhs, :rhs rhs} tokens)
      {:type :op :op op, :lhs lhs, :rhs rhs})
  ))

;----------------------
(defn multiplicative-expression [tokens]
  (let*
    [
     unary (unary-expression tokens)
     ]
    (if (is-multop (peek-token tokens))
      (multiplicative-tail unary tokens)
      unary)))

;----------------------
(defn additive-tail [lhs tokens]
  (let*
    [
     op (next-token tokens)
     rhs (multiplicative-expression tokens)
     ]
    (if (is-addop (peek-token tokens))
      (additive-tail {:type :op :op op, :lhs lhs, :rhs rhs} tokens)
      {:type :op :op op, :lhs lhs, :rhs rhs})
  ))

;----------------------
(defn additive-expression [tokens]
  (let*
    [
     mult (multiplicative-expression tokens)
     ]
    (if (is-addop (peek-token tokens))
      (additive-tail mult tokens)
      mult)))

;----------------------
(defn relational-tail [lhs tokens]
  (let*
    [
     op (next-token tokens)
     rhs (additive-expression tokens)
     ]
    (if (is-relop (peek-token tokens))
      (relational-tail {:type :op :op op, :lhs lhs, :rhs rhs} tokens)
      {:type :op :op op, :lhs lhs, :rhs rhs})
  ))

;----------------------
(defn relational-expression [tokens]
  (let*
    [
     add (additive-expression tokens)
     ]
    (if (is-relop (peek-token tokens))
      (relational-tail add tokens)
      add)))

;----------------------
(defn equality-tail [lhs tokens]
  (let*
    [
     op (next-token tokens)
     rhs (relational-expression tokens)
     ]
    (if (is-eqop (peek-token tokens))
      (equality-tail {:type :op :op op, :lhs lhs, :rhs rhs} tokens)
      {:type :op :op op, :lhs lhs, :rhs rhs})
  ))

;----------------------
(defn equality-expression [tokens]
  (let*
    [
     rel-exp (relational-expression tokens)
     ]
    (if (is-eqop (peek-token tokens))
      (equality-tail rel-exp tokens)
      rel-exp)))

;----------------------
(defn logical-and-tail [lhs tokens]
  (let*
    [
     op (next-token tokens)
     rhs (equality-expression tokens)
     ]
    (if (is-logand (peek-token tokens))
      (logical-and-tail {:type :op :op op, :lhs lhs, :rhs rhs} tokens)
      {:type :op :op op, :lhs lhs, :rhs rhs})
  ))

;----------------------
(defn logical-and-expression [tokens]
  (let*
    [
     eq-exp (equality-expression tokens)
     ]
    (if (is-logand (peek-token tokens))
      (logical-and-tail eq-exp tokens)
      eq-exp)))

;----------------------
(defn logical-or-tail [lhs tokens]
  (let*
    [
     op (next-token tokens)
     rhs (equality-expression tokens)
     ]
    (if (is-logor (peek-token tokens))
      (logical-or-tail {:type :op :op op, :lhs lhs, :rhs rhs} tokens)
      {:type :op :op op, :lhs lhs, :rhs rhs})
  ))

;----------------------
(defn logical-or-expression [tokens]
  (let*
    [
     and-exp (logical-and-expression tokens)
     ]
    (if (is-logor (peek-token tokens))
      (logical-or-tail and-exp tokens)
      and-exp)))

;----------------------
(defn conditional-expression [tokens]
  (logical-or-expression tokens))

;----------------------
(defn assignment-expression [tokens]
  (if (is-assignment (peek-token2 tokens))
      (assignment-expr (next-token tokens) tokens)
      (conditional-expression tokens)))

;----------------------
(defn expression [tokens]
  (assignment-expression tokens))

;----------------------
(defn variable-statement [tokens stmt]
  (let*
    [
     token (peek-token tokens)
     ]
    (if (and
          (is-keyword token)
          (= (get token :val) VAR))
      (do
        (advance tokens)
        (let*
          [
           args (variable-declaration-list tokens)
           ]
          (var-set stmt {:type :var, :args args})
          (consume-punctuator tokens SEMI)
          true))
      false)))

;----------------------
(defn empty-statement [tokens stmt]
  (if (is-end-of-stmt (peek-token tokens))
    (do
      (var-set stmt {:type :empty})
      (consume-punctuator tokens SEMI)
      true)
    false))

;----------------------
(defn expression-statement [tokens stmt]
  (var-set stmt (expression tokens))
  (consume-punctuator tokens SEMI)
  true)

;----------------------
(defn else-branch [tokens]
  (if (is-keyword-else (peek-token tokens))
    (do
      (advance tokens)
      (statement tokens))
    nil))

;----------------------
(defn if-statement [tokens stmt]
  (if (is-keyword-if (peek-token tokens))
    (do
      (advance tokens)
      (consume-punctuator tokens LPAREN)
      (let*
        [
         logexp (expression tokens)
         ]
        (consume-punctuator tokens RPAREN)
        (let*
          [
           true-stmt (statement tokens)
           false-stmt (else-branch tokens)
           ]
          (var-set stmt {:type :if, :logexp logexp, :true true-stmt :false false-stmt})
          true)))
      false))

;----------------------
(defn do-statement [tokens stmt]
  (if (is-keyword-do (peek-token tokens))
    (do
      (advance tokens)
      (println "do-statement-1" @tokens)
      (with-local-vars
        [
         block-stmt nil
         ]
        (block tokens block-stmt)
        (println "do-statement-2" @tokens)
        (advance tokens)
        (println "do-statement-3" @tokens)
        (consume-punctuator tokens LPAREN)
        (let*
          [
           logexp (expression tokens)
           ]
          (consume-punctuator tokens RPAREN)
          (var-set stmt {:type :do-while, :logexp logexp, :body @block-stmt})
          (println @stmt)
          true)))
    false))

;----------------------
(defn while-statement [tokens stmt]
  (if (is-keyword-while (peek-token tokens))
    (do
      (advance tokens)
      (consume-punctuator tokens LPAREN)
      (let*
        [
         logexp (expression tokens)
         ]
        (consume-punctuator tokens RPAREN)
        (let*
          [
           body-stmt (statement tokens)
           ]
          (var-set stmt {:type :while, :logexp logexp, :body body-stmt})
          true)))
      false))

;----------------------
(defn for-statement [tokens stmt]
  (if (is-keyword-for (peek-token tokens))
    (do
      (advance tokens)
      (consume-punctuator tokens LPAREN)
      (let*
        [
         init-expr (expression tokens)
         semi-1 (consume-punctuator tokens SEMI)
         limit-expr (expression tokens)
         semi-2 (consume-punctuator tokens SEMI)
         update-expr (expression tokens)
         ]
        (consume-punctuator tokens RPAREN)
        (let*
          [
           body-stmt (statement tokens)
           ]
          (var-set stmt
                   {:type :for,
                    :init-expr init-expr,
                    :limit-expr limit-expr,
                    :update-expr update-expr,
                    :body-stmt body-stmt
                    })
          true)))
      false))

;----------------------
(defn iteration-statement [tokens stmt]
  (or
    (while-statement tokens stmt)
    (do-statement tokens stmt)
    (for-statement tokens stmt)))

;----------------------
(defn block [tokens stmt]
  (if (is-left-brace (peek-token tokens))
    (do
      (consume-punctuator tokens LBRACE)
      (with-local-vars
        [
         blocks []
         ]
        (loop []
          (let*
            [
             block
             (statement tokens)
             ]
            (if (not (nil? block))
              (var-set blocks (conj @blocks block)))
            (if (is-right-brace (peek-token tokens))
              (do
                (consume-punctuator tokens RBRACE)
                (var-set stmt {:type :block, :stmts @blocks})
                true)
              (if (not (empty? @tokens))
                (recur)
                false))))))
    false))

;----------------------
(defn statement [tokens]
  (with-local-vars
    [stmt nil]
    (or
      (block tokens stmt)
      (variable-statement tokens stmt)
      (if-statement tokens stmt)
      (iteration-statement tokens stmt)
      (empty-statement tokens stmt)
      (expression-statement tokens stmt))
    @stmt))

;----------------------
(defn action-element [tokens]
  (statement tokens)
  )

;----------------------
(defn action-elements [tokens]
  (with-local-vars
    [
     actions []
     ]
    (loop []
      (let*
        [
         action
         (action-element tokens)
         ]
        (if (nil? action)
          (var-set tokens [])
          (var-set actions (conj @actions action))))
      (if (and
            (not (empty? @tokens))
            (not (is-right-brace (peek-token tokens))))
        (recur)))
    @actions))

;----------------------
(defn process-declaration [tokens]
  (let*
    [
     process-name (identifier tokens)
     dummy1 (consume-punctuator tokens COLON)
     dummy2 (consume-punctuator tokens LBRACE)
     actions (action-elements tokens)
     ]
     (consume-punctuator tokens RBRACE)
     (consume-punctuator tokens SEMI)
    {
     :type :process,
     :name process-name,
     :actions actions
     }))

;----------------------
(defn task-declaration [tokens]
  (let*
    [
     dummy1 (consume-punctuator tokens LBRACE)
     task-name (identifier tokens)
     dummy2 (consume-punctuator tokens RBRACE)
     actions-kwd (next-token tokens)
     dummy3 (consume-punctuator tokens LBRACE)
     action-name (identifier tokens)
     dummy4 (consume-punctuator tokens COLON)
     via-kwd (next-token tokens)
     action-expr (function-expr action-name tokens)
     ]
     (consume-punctuator tokens RBRACE)
     (consume-punctuator tokens SEMI)
    {
     :type :task,
     :name task-name,
     :action action-expr
     }))

;----------------------
(defn nblo-declaration [tokens]
  (let*
    [
     nblo-name (identifier tokens)
     args (formal-args tokens)
     ]
    (if (is-native (peek-token tokens))
      (let*
        [
         native (next-token tokens)
         code (get native :val)
         exprs (read-string (format "[%s]" code))
         ]
        (consume-punctuator tokens SEMI)
        {
         :type :nblo,
         :args args,
         :exprs exprs,
         :name nblo-name
         })
      (let*
        [
         expr (error-expr "nblo-declaration" tokens)
         ]
        (var-set tokens [])
        expr))))

;----------------------
(defn source-element [tokens]
  (cond
    (is-keyword-process (peek-token tokens))
    (do
      (advance tokens)
      (process-declaration tokens))
    (is-keyword-task (peek-token tokens))
    (do
      (advance tokens)
      (task-declaration tokens))
    (is-keyword-nblo (peek-token tokens))
    (do
      (advance tokens)
      (nblo-declaration tokens))
    true
    [])
  )

;----------------------
(defn source-elements [tokens src-elements]
  (loop []
    (let*
      [
       src-element
       (source-element tokens)
       ]
      (if (nil? src-element)
        (var-set tokens [])
        (var-set src-elements (conj @src-elements src-element))))
    (if (not (empty? @tokens))
      (recur))))

;----------------------
(defn script [script]
  (with-local-vars
    [
     tokens (tokenize script)
     src-elements []
     ]
    (source-elements tokens src-elements)
    @src-elements))
