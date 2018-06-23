
/// a vector of features that we want to maximize
type Vector = float array
/// human anotated data denoting a comparaison
type Sample = {winner : Vector; loser : Vector}
/// syntaxic suggar to build a sample (converts sequence type to array)
let (>.) winner loser = {winner = Seq.toArray winner; loser = Seq.toArray loser}

//-----------------------------------------------------------------------------
// VECTOR OPERATIONS

/// computes v1 - v2
let sub v1 v2 : Vector = 
   Array.map2 (-) v1 v2

/// computes v1 + v2
let add v1 v2 : Vector =
   Array.map2 (+) v1 v2

/// computes v / lambda
let div lambda v =
   Array.map (fun x -> x / lambda) v

/// computes the average of a sequence of vectors
let average vectors =
   let length = vectors |> Seq.length |> float
   vectors |> Seq.reduce add |> div length

/// computes the dot product between v1 and v2
/// Note : loops could be fused o increase performances but performance is not the point here
let dotProduct (v1 : Vector) (v2 : Vector) =
   Array.map2 (*) v1 v2 |> Array.reduce (+)

//-----------------------------------------------------------------------------
// TRAINING

/// computes winners - losers
let computesDifferences data =
   Seq.map (fun s -> sub s.winner s.loser) data

/// makes sure that a score is legal
let legalizeWeights weigts =
   let weigts = Array.map (max 0.) weigts // makes sure that all coordinates are >= 0
   match Array.forall ((=) 0.) weigts with
   | true -> Array.create weigts.Length 1. // default value in the absence of informations
   | false -> weigts

/// computes the weights for the scoring
let computeScore trainingData =
   trainingData
   // |> normalize
   // |> keepOnlyNonTrivialComparaisons // the usefullness of this step is debatable
   |> computesDifferences // computes a sequence of differences
   |> average // averages the differences to get raw weights (a median might be a more robust decision)
   |> legalizeWeights // insures that the weights are legal

//-----------------------------------------------------------------------------
// BIGGER

/// return v1 >= v2 in the pareto sence (for all coordinates)
/// None means that it cannot be decided strictly
let paretoBigger v1 v2 =
   if Array.forall2 (>=) v1 v2 then Some true
   elif Array.forall2 (<) v1 v2 then Some false
   else None

/// return score(v1) >= score(v2)
/// Note : we could factor the dot product to improve performances but performance is not the point here
let scoreBigger weights v1 v2 =
   let score1 = dotProduct weights v1
   let score2 = dotProduct weights v2
   score1 >= score2

/// compares two vectors using pareto inequality if possible
/// otherwise fall back to a score computed using the given weights
let bigger weights v1 v2 =
   match paretoBigger v1 v2 with
   | Some result -> result
   | None -> scoreBigger weights v1 v2

/// takes training data and returns a comparaison function
let buildComparaison trainingData =
   let weights = computeScore trainingData
   printfn "The weights are %A" weights
   bigger weights

//-----------------------------------------------------------------------------
// DEMO

/// displays a test
let test comparaison v1 v2 =
   printfn "%A >= %A ? %b" v1 v2 (comparaison v1 v2)

let trainingData =
   [|
      [|1.;3.;5.|] >. [|2.;2.;2.|]
      [|90.;45.;1.|] >. [|80.; 35.; 200.|]
   |]

let comparaison = buildComparaison trainingData
test comparaison [|1.;3.;5.|] [|2.;2.;2.|]
test comparaison [|90.;45.;1.|] [|80.; 35.; 200.|]
test comparaison [|2.; 5.; 9.|] [|6.; 7.; 8.|]