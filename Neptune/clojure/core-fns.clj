;;;--------------------
;;; Neptune Core Functions
;;;--------------------

;----------------------
(def neptune-methods (atom {}))

;----------------------
(defn set-method [name fn-block]
  (swap!
    neptune-methods
    (fn [_]
      (assoc
        @neptune-methods
        (keyword name)
        fn-block))))

;----------------------
(defn nsfn [name & args]
  (let*
    [
     fn (get @neptune-methods (keyword (str name)))
     ]
    (if (fn? fn)
      (apply fn args))))

;----------------------
(defn nsfn-str [name & args]
  (with-out-str
    (apply nsfn (cons name args))))

;----------------------
(defn is_member [elem ilist]
  (cond (empty? ilist) false
        (= elem (first ilist)) true
        :else (recur elem (rest ilist))))

;----------------------
(defn show-msg [msg]
	(System.Windows.Forms.MessageBox/Show (str msg)))

;----------------------
(defn pddl-run [name, dir, dfile, pfile]
	(Neptune.NeptuneIDE/pddlRun (str name) (str dir) (str dfile) (str pfile)))

;----------------------
(defn print-transcript [text]
	(Neptune.NeptuneIDE/printTranscript text))

;----------------------
(defn print-info [text]
	(Neptune.NeptuneIDE/printInfo text))

;----------------------
(defn print-output [text]
	(Neptune.NeptuneIDE/printOutput text))

;----------------------
(defn print-error [text]
	(Neptune.NeptuneIDE/printError text))

;----------------------
(defn sorted-keys [expr]
	(vec (sort (for [k (keys expr)] (.Substring (str k) 1)))))

;----------------------
(defn seq-as-arraylist [seq]
	(let*
	[
	array (System.Collections.ArrayList.)
	]
	(doseq [x seq]
		(.Add array x))
	array))
