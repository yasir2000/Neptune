(define (domain petshop)
  (:requirements :strips)
  (:predicates
   (at ?thing ?place) (driving ?p ?v)
   (mobile ?thing) (person ?p) (pet ?v))

  (:action drive
    :parameters (?thing ?from ?to)
    :precondition (and (mobile ?thing) 
		       (at ?thing ?from))
    :effect (and (at ?thing ?to) (not (at ?thing ?from))))

  (:action purchase
    :parameters (?person ?place ?pet)
    :precondition (and (person ?person) (pet ?pet)
		       (at ?person ?place) (at ?pet ?place))
    :effect (and (driving ?person ?pet) (mobile ?pet)
		 (not (at ?person ?place)) (not (mobile ?person))))

  (:action disembark
    :parameters (?person ?place ?pet)
    :precondition (and (person ?person) (pet ?pet)
		       (driving ?person ?pet) (at ?pet ?place))
    :effect (and (at ?person ?place) (mobile ?person)
		 (not (driving ?person ?pet)) (not (mobile ?pet))))
  )
 