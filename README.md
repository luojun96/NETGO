# NetGo

NetGo is aimed to set up a generic CI/CD flow for Open Source .Net Core Project.

In this project, we will use GitHub to store source code, and Travis CI to build main CI/CD flow. What's more. we use C# DSL Tool Cake to define the local CI flow.

For CI, we should ensure the checked code is merged into main branch when all CI passed.
The CI flow is consisted by following stages:
- Clean 
- Version
- Compile
- UnitTest
- Create Packages
    - Nuget
    - Docker Images
    - Helm Chart(Optional)
- SanityTest
    - Deploy origin test environment
    - Run Sanity Tests
- Submit Code

Use Travis CI stages(install, script) to do test.
Build docker images and push to docker registry in CI.
For CD, I consider to deploy to public cloud environment, such as: Azure, AWS and herokou

