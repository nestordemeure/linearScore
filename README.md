# linearScore

Computes a linear scoring function for multicriteria optimization (with an emphasis on simplicity of method).

## Problem statement

We have objects described by several evaluations.
We are trying to **maximise** those evaluations.
Given two objects, when all evaluations agree it is easy to establish the better one (pareto dominance).
Otherwise we need a way to make our choice.  

Given a database of cases in which a humans choose the best of two or more options, we want to establish a linear combinaison of the evaluations that best approximate the unknown human criteria (pareto front).
Note that this is not a classic formulation of a regression or classification problem.  

Our solution being linear it is only an approximation of the pareto front but it is very easy to compute, optimal (among linear approximation) and can be updated on the fly given new human data.

## Algorithm

- normalize the units of the data (if dealing with lenght and area, take the sqrt of the areas).
- normalize the data relative to the problem (if a quantity growths linearly with the problem size, divide it by the problem size to insure that you will be able to transfer knownledge from your training problems to your testing problems).
- Collect the couples (winner,loser) that are not trivial to decide (pareto dominance of the winner over the loser).
- Compute their differences `dif = winner - loser`.
- Average the coordinates of the differences.
- if the average has a negative coordinate, fix it to zero (to force the average into the pareto dominance area).
- if all the coordinates of the averages are 0 then fixe them to 1 (we have no information : equal weight for all score).
- the coordinates are the weights for our scoring function

To compare two solutions, we first check if one of them is pareto dominant, otherwise we use our scoring function.

## Notes

The algorithm relies on the fact that the score is linear meaning that `score(x) > score(y) => score(x - y) > 0`.  

The problem is simetrical, `score(diff) > 0 => score(-diff) < 0` which mean that we could see it as the search for a linear separation between the differences and their opposite (the problem can be seen as a particular a classification problem).  

A very simple linear separator between two class is the line equidistant to their respective mean.
Here it translates to a separator orthogonal to the vector going from the origin to the mean of the differences.
Meaning that we can use the coordinate of the mean as the score we are looking for.
For small smaple sizes the median might be a more robust estimator, it might be worth exploring (but if we accept to increase the complexity here why not go all the way and use a soft margin minimizer...).  

We use only non trivially separable couples because the other couples are in a quadrant that contains no informations and hence can be ignored (as long as we can recover from a mean that is not in this same quadrant).
But this choice is debatable.

## References

I found no such algorithm in the litterature but it seems highly probable that it is only a reinvention of an existing idea.