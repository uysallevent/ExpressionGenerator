# ExpressionGenerator
This generic class, can be used to generate expression for according to model. 
This metod generated with using reflection. All you have to do is adding the model you want to expression to the parameters.

This class accept 2 generic models. If your input and output models are the same, you can give the same model in both. But When your input and output model are different,  properties names must be same. (You prevent unwanted properties from appearing on the output )


# Important
1) If input and output model are different, properties names must be same but not all properties are required.(only the fields you want to show in the answer)
