;;;--------------------
;;; Neptune Goal Functions
;;;--------------------

;----------------------
(defn goals [script]
;----------------------
	(if (neptune-script? script)
		(get script :goals {})
		{}))

;----------------------
(defn goal-keys-array []
;----------------------
	(seq-as-arraylist (sorted-keys (goals neptune-script))))

;----------------------
(defn goal-get-name [name]
;----------------------
	(get (goals neptune-script) (keyword name) {}))

;----------------------
(defn goal-get-title [name]
;----------------------
	(get (goal-get-name name) :title ""))

;----------------------
(defn goal-get-notes [name]
;----------------------
	(get (goal-get-name name) :notes ""))

;----------------------
(defn goal-save-text [name title notes]
;----------------------
	(let*
	[
		goals (assoc (goals neptune-script) (keyword name) {:title title, :notes notes})
	]
	(def neptune-script (assoc neptune-script :goals goals))
	(format "goal [%s] saved." name)))

;----------------------
(defn goal-delete-text [name]
;----------------------
	(let*
	[
		goals (dissoc (goals neptune-script) (keyword name))
	]
	(def neptune-script (assoc neptune-script :goals goals))
	(format "goal [%s] deleted." name)))

