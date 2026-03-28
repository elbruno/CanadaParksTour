var builder = DistributedApplication.CreateBuilder(args);

// Add the API project
var api = builder.AddProject<Projects.OntarioParksExplorer_Api>("api")
    .WithExternalHttpEndpoints();

// Add the Blazor project with reference to API
var blazor = builder.AddProject<Projects.OntarioParksExplorer_Blazor>("blazor")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

// Add the React project (Vite-based, using npm dev script)
var react = builder.AddNpmApp("react", "../OntarioParksExplorer.React", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
