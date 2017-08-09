;;;--------------------
;;; Neptune Method Functions
;;;--------------------

;----------------------
(defn clear-cache []
;----------------------
  (def neptune-cache {})
)

;----------------------
(defn get-methods [script]
;----------------------
	(if (neptune-script? script)
		(get script :methods {})
		{}))

;----------------------
(defn lookup-cache [selector]
;----------------------
	(get neptune-cache selector nil))

;----------------------
(defn lookup-method [selector]
  (let*
    [
     cached-method (lookup-cache selector)
     ]
    (if (not (nil? cached-method))
      cached-method
         (let*
          [
           new-method (get (get-methods neptune-spc) selector nil)
           new-method-fn (eval new-method)
           ]
          (def neptune-cache (assoc neptune-cache selector new-method-fn))
          new-method-fn))))

;----------------------
(defn invoke-method [selector args]
;----------------------
	(let*
   [
    method (lookup-method selector)
    method-fn (eval method)
    ]
   (if (fn? method-fn)
     (apply method-fn args)
     nil)))
		
;----------------------
(defn run-method [selector args]
;----------------------
	(invoke-method (keyword selector) args))

;----------------------
(defn neptune-run []
;----------------------
  (clear-cache)
	(print-transcript (format "Script [%s] started..." (neptune-script-name)))
	(let*
		[
		output
		(with-out-str
			(run-method :start []))
		]
		(print-output output)
		nil)
	(print-transcript (format "Script [%s] ended." (neptune-script-name))))

;----------------------
(defn methods-for-script [name]
;----------------------
	(get-methods (neptune-get-script name)))

;----------------------
(defn method-keys-array [name]
;----------------------
	(seq-as-arraylist (sorted-keys (methods-for-script name))))

;----------------------
(defn methods-format [src]
;----------------------
	(try
		(pprint (read-string src))
	(catch Exception e
		src))
)

;----------------------
(defn methods-get-selector [name selector]
;----------------------
  (let*
    [
     method-fn
     (get (methods-for-script name) (keyword selector) nil)
     ]
    (if (nil? method-fn)
      "-- not found --"
      (pprint method-fn))))

;----------------------
(defn methods-save-selector [name selector src]
;----------------------
	(let*
	[
  method (read-string src)
  methods (assoc (methods-for-script name) (keyword selector) method)
  script (assoc (neptune-get-script name) :methods methods)
	]
	(neptune-set-script name script)
	(format "Method [%s] saved." selector)))

;----------------------
(defn methods-delete-selector [name selector]
;----------------------
	(let*
	[
  methods (dissoc (methods-for-script name) (keyword selector))
  script (assoc (neptune-get-script name) :methods methods)
	]
	(neptune-set-script name script)
	(format "Method [%s] deleted." selector)))

;----------------------
(defn self [selector & args]
;----------------------
  (run-method selector (vec args))
)
