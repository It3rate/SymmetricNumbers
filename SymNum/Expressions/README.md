# The Decision Heuristic

## Straps
- Properties are connected by 'Straps', which is a gradient between two values.
- The properties can be the same type (like a fraction) or different types (like a ratio)
- The values can be two measures, two halves of a number (real an imag), two numbers, or two expressions (these are all numbers)
- The gradient is just the potential, it can contain different Numbers that are inputs, outputs, expectations, measures etc.

## Clusters
- A set of properties can be interconnected with each other, either directly, or a->b->c 
- A connection can flow in either direction, or back and forth, depending how they are connected, and to what
- A cluster that solves a specific problem is a concept, but there are no hard bounds to these (a light sensor can solve many problems)

## The Single Number Type
- Our numbers are equations, in the sense that they are n/1 (a divison by unit). All numbers are equations.
- Multiple numbers combined with ops are also equations, and can be treated like numbers (or numbers can be treated like equations, same thing)
- Partial numbers are uncalibrated in some way, like x/1 or 7/x. Partial equations as well.
- An equation always has a result, even if it is unknowable or imprecise.
- An equation also has an expectation, or a desired output, or a prediction. The value before you measure the result. 
- The measured result and the expected result will either be the same, or have an error/diff/suprise
- This error is at simplest a true or false, but can be calibrated into anything that goes in an if statement
- In language the error is like a preposition (near, between, after, behind etc.) In math it's an expression x>10 && x<15 etc.

## The Algorithm
- light, muscle and hunger are three properties hooked to each other by straps
- light is input, so one direction, but it still can predict.
- hunger is internal state, so it is input that is casued (by lack of food over time) and controllable (with food)
- muscle is output, it can be self controlled or controlled by other states, and its actions directly affect input (light).
- predict, act, measure, diff -> error correct by making new prediction
- e.g. predict next light level, move muscles, measure light, diff.

## Predictions
- the advantage of predict->measure rather than measure->measure->find pattern, is the first requires no memory
- prediction isn't just expectation or estimation, it is how goals are set. You can predict motion and that will cause you to move.
- prediction is set by what has focus (can be shared). muscle, eyes and hunger can all predict/expect/desire next light value
- The gradients that have focus collectivley predict/set desire for the next light. 
    - If hungry, desire light
    - if scared desire darkness quickly
    - if neutral predict next light based on previous samples
- There are many aspects of a concept, but we only use a few at once. It is these subsets that determine the prediction.
- a prediction can have agency. The leg can predict speed because it knows it will push.

## Measuring the result
- Sometimes you just need true or false (light will get brighter)
- Somemtimes you want quantification or ranges (light will get 10% to 20% brighter)
- Sometimes you want relative information (light will be outside, touching, near current range)
- Sometimes you want 16 bool options (prediction and actual have no overlap)
- A number can represent all of these, and subtracting (or other op) the prediction from the actual gives you this number.