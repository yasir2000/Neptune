;;;--------------------
;;; Neptune IDE Functions
;;;--------------------

;----------------------
(defn eval-string [s]
;----------------------
	(with-out-str (pr 
		(try
			(eval (read-string s))
		(catch Exception e
			(println (.Message e)))))))

;----------------------
(defn delete-script []
;----------------------
	(def neptune-script nil))

;----------------------
(defn new-script []
;----------------------
	(def neptune-script
		{
			:name "New Script",
			:type :neptune-script,
			:level :spc,
			:desc ""
		}))

;----------------------
(defn read-script [name]
;----------------------
	(def neptune-script nil)
	(def neptune-script
		(read-string (slurp (format "../../neptune/%s.clj" name) :encoding "ascii")))
	(pr-str neptune-script))

;----------------------
(defn save-script [name]
;----------------------
	(if (neptune-script? neptune-script)
		(do
			(spit (format "../../neptune/%s.clj" name) (pr-str neptune-script))
			(show-msg (format "Script [%s] saved." name)))
		(show-msg "No script loaded.")))

;----------------------
(defn load-script [name]
;----------------------
	(let* [msg
		(try
			(read-script name)
			(if (neptune-spc? neptune-script)
				(do
					(def neptune-spc neptune-script)
					(print-spc-info)
					(print-transcript (format "Neptune SPC [%s] loaded" name)))
				(print-transcript (format "Script [%s] loaded" name)))
		(catch Exception e
			(def frame-error e)
			(print-error (format "Error loading script [%s] [%s]" name (.get_Message e)))))
		]
		(show-msg msg)))

;----------------------
(defn print-spc-info []
;----------------------
	(let*
		[
		name (neptune-script-name)
		goal-keys (sorted-keys (goals neptune-script))
		method-keys (sorted-keys (get-methods neptune-script))
		nblo-keys (sorted-keys (nblos neptune-script))
		text
		(with-out-str
			(println "load successful")
			(println "")
			(println (format "Name: %s" name))
			(println "")
			(println "Goals:")
			(doseq [name goal-keys] (println "--" name))
			(println "")
			(println "Methods:")
			(doseq [name method-keys] (println "--" name))
			(println "")
			(println "NBLOs:")
			(doseq [name nblo-keys] (println "--" name))
			(println ""))
		]
		(print-transcript text)))
