ReTry gives you the ability to have service calls retried n-number of times before failing.

This can be useful when making calls over the internet or to a database and you don't want a temporary interuption to raise an exception without first trying a couple of more times.

ReTry lets you decide how to handle specific exception types if the service fails so that you can perform different actions.

Uses a fluent interface so you can do stuff like:

    // Make a service call and try 3 times if an exception is raised. After 3 attempts, return a redirect.
	var result = reTry
                    .ExecuteService<ServiceResult, RedirectResult>(() => someService.MakeHttpCall("Foo","Bar") , 3)
                    .IfServiceFailsThen<HttpFailedException>(() => RedirectSomewhereElse())
                    .Result();