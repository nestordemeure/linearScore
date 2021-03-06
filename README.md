# linearScore

Computes a linear scoring function for multicriteria optimization (with an emphasis on simplicity of method).

## Problem statement

We have objects described by several criteria. We are trying to **maximise** those criteria.
Given two objects, when all evaluations agree it is easy to establish the better one (pareto dominance).
Otherwise we need a way to make our choice.  

Given a database of cases in which a humans choose the best of two or more options, we want to establish a linear combinaison of the criteria that best approximate the unknown human criteria (pareto front).  

Our solution being linear it is only an approximation but it is very easy to compute and can be updated on the fly given new human data.

## Algorithm

- normalize the units of the data (if dealing with lenght and area : take the sqrt of the areas, etc).
- normalize the data relative to the problem (if a quantity growths linearly with the problem size, divide it by the problem size to insure that you will be able to transfer knownledge from your training samples to your testing samples).
- Collect the couples (winner,loser) that are not trivial to decide (no pareto dominance of the winner over the loser).
- Compute their differences `dif = winner - loser`.
- Average the coordinates of the differences.
- if the average has a negative coordinate, fix it to zero (to force the average into the pareto dominance area).
- if all the coordinates of the average are 0 then fixe them to 1 (we have no information : equal weight for all score).
- the coordinates are the weights for our scoring function

To compare two solutions, we first check if one of them is pareto dominant, otherwise we use our scoring function.

## Notes

The algorithm relies on the fact that the score is linear meaning that `score(x) > score(y) => score(x - y) > 0`.  

The problem is symetrical, `score(diff) > 0 => score(-diff) < 0` which mean that we could see it as the search for a linear separation between the differences and their opposite (the problem can be seen as a particular a classification problem).  

A very simple linear separator between two class is the line equidistant to their respective mean.
Here it translates to a separator orthogonal to the vector going from the origin to the mean of the differences.
Meaning that we can use the coordinate of the mean as the score we are looking for. 

We only use datapoints with no pareto dominance since they are the only one to reflect the human criteria (being non trivially separable). This design choice is debatable.

## References

I found no such algorithm in the litterature but it seems highly probable that it is only a reinvention of an existing idea.