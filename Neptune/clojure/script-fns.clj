;;;--------------------
;;; Neptune Script Functions
;;;--------------------

;----------------------
; Declarations
;----------------------
(declare print-spc-info)

;----------------------
; Globals
;----------------------
(def neptune-cache nil)
(def neptune-script nil)
(def neptune-spc nil)
(def frame-error nil)

;----------------------
(defn neptune-get-script [name]
;----------------------
	(eval (symbol name)))

;----------------------
(defn neptune-set-script [name script]
;----------------------
	(eval (list 'def (symbol name) (list 'quote script))))

;----------------------
(defn neptune-script? [expr]
;----------------------
	(and (map? expr) (= (get expr :type nil) :neptune-script)))

;----------------------
(defn neptune-spc? [expr]
;----------------------
	(and (neptune-script? expr) (= (get expr :level nil) :spc)))

;----------------------
(defn neptune-script-attr [key default]
;----------------------
	(if (not (neptune-script? neptune-script))
		default
		(get neptune-script key default)))

;----------------------
(defn neptune-script-name []
;----------------------
	(neptune-script-attr :name "Undefined"))

;----------------------
(defn neptune-script-set-name [name]
;----------------------
	(if (neptune-script? neptune-script)
		(def neptune-script (assoc neptune-script :name (str name)))))

;----------------------
(defn neptune-script-level []
;----------------------
	(neptune-script-attr :level :none))

;----------------------
(defn neptune-script-level-str []
;----------------------
	(.Substring (str (neptune-script-level)) 1))

;----------------------
(defn neptune-script-type []
;----------------------
	(neptune-script-attr :type nil))

;----------------------
(defn neptune-script-type-str []
;----------------------
	(.Substring (str (neptune-script-type)) 1))

;----------------------
(defn neptune-script-desc []
;----------------------
	(neptune-script-attr :desc ""))

;----------------------
(defn neptune-script-set-desc [desc]
;----------------------
	(if (neptune-script? neptune-script)
		(def neptune-script (assoc neptune-script :desc (str desc)))))

;----------------------
(defn neptune-script-source []
;----------------------
	(pr-str neptune-script))
