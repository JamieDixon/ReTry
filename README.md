It's a service manager that needs a much better name o_0

Basically this class lets you pass it a method to execute and it tries n number of times to execute the method.

If after n times it is still throwing an exception it falls back to executing the methods you define in the IfServiceFailsThen method.

Uses a fluent interface so you can do stuff like:

	var result = serviceManager
                    .ExecuteService(() => someService.MakeHttpCall("Foo","Bar") , 3)
                    .IfServiceFailsThen((exception) => new SomeServiceDefaultResult())
                    .Result<SomeServiceResult>();