Implementing OAuth 2.0 (Machine - Machine - Machine) using .Net Core Web Api
- Api.Application will send Request to Api.Client(Api authentication using MS.Identity)
- Api.Client will Authenticate the Api.Application request (using Principal Claims and Secret key)
- Api.Client will send response with JWT Token
- Api.Application can now send Request with Api.Client(with Bearer JWT Token) to access an endpoint
- Api.Client will Validate the token
- Api.Client will send a request to the Api.Server to validate connecting machine(Api.Client)
- Api.Server will authenticate credentials of the machine(client_id and secret key)
- Api.Server will send back with JWT Token
- Api.Client will receive a valid token

Technologies used:

-.Net Core Api 8.0 
- OpenIddict
- MS.Identity
- Entity Framework
