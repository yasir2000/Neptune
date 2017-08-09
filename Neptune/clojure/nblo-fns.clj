;;;--------------------
;;; Neptune BLO Functions
;;;--------------------

;----------------------
(defn nblos [script]
;----------------------
	(if (neptune-script? script)
		(get script :nblos {})
		{}))

;----------------------
(defn lookup-nblo [script selector]
;----------------------
	(get (nblos script) selector nil))

;----------------------
(defn invoke-nblo [script selector args]
;----------------------
	(let*
	[
		nblo (lookup-nblo script selector)
	]
	(if (string? nblo)
		(apply (eval (read-string nblo)) args)
		nil)))
		
;----------------------
(defn callnblo [name & args]
;----------------------
	(invoke-nblo neptune-spc (keyword name) args))

;----------------------
(defn nblo-keys-array []
;----------------------
	(seq-as-arraylist (sorted-keys (nblos neptune-script))))

;----------------------
(defn nblo-get-name [name]
;----------------------
	(get (nblos neptune-script) (keyword name) ""))

;----------------------
(defn nblo-save-src [name src]
;----------------------
	(let*
	[
		nblos (assoc (nblos neptune-script) (keyword name) src)
	]
	(def neptune-script (assoc neptune-script :nblos nblos))
	(format "nblo [%s] saved." name)))

;----------------------
(defn nblo-delete-src [name]
;----------------------
	(let*
	[
		nblos (dissoc (nblos neptune-script) (keyword name))
	]
	(def neptune-script (assoc neptune-script :nblos nblos))
	(format "nblo [%s] deleted." name)))

