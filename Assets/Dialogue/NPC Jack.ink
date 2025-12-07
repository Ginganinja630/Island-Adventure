EXTERNAL VerifyQuest()
VAR questCompleted = false

-> start

=== start ===
A long time ago, a couple of orcs stole my candy.
It would be nice if I could get it back.
By chance, have you come across my candy?
    * [Yes]
        ~ VerifyQuest()
        { questCompleted:
            -> success
        - else :
            -> failure
        }
    * [No]
        -> noCandy
-> END

=== noCandy ===
Come back if you find my candy.
-> END

=== success ===
Thank you so much!  Here's a reward.
-> END

=== failure ===
Looks like you don't have it.  Come back if you find my candy.
-> END

=== postCompletion ===
Thanks for helping me earlier.  Good luck with your adventures!
-> END