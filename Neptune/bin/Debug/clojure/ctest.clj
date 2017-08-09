(ns clojure.ctest)

(def factions
    [])

(defn list-available-public-reports [] nil)

(defn save-report
    ([turn ^String faction-id] (save-report turn faction-id nil))
    ([turn ^String faction-id ^String password] nil))

(defn save-reports
    "Downloads the turn reports for all factions up to and including the given turn that weren't already downloaded in the reports folder.
     Returns the number of downloaded reports."
    [^long until-turn]
    (let     [down-one (fn [[turn faction-id faction-password]] 
	                         (if (save-report turn faction-id faction-password) 1 0))
             turns-factions (for [turn (range 1 (inc until-turn)) faction factions] [turn (:id faction) (:password faction)])]
      (reduce + (pmap down-one (concat turns-factions
                                    (map #(concat % [nil])
                                          (list-available-public-reports)))))))
