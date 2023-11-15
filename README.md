# Retirement home
> The set of tools, apps and infrastructure for monitoring health and wellbeing of retirement home residents

## Abbriviations:
### Actors
- RH = Retirement Home
- R = Resident
- N = Nurse (in RH)
- FM = Family Member
- D = Doctor
- CO = Content owner in RH (person who manages info about RH and R activites)

### Tech
- GA = Google Api (mostly Fitness)
- MTApp = Mobile/Tablet application for 

## Key function of the solution:
- Smart band colects R health data
- Data is viewable for FM who can monitor health of his/her relative
- N gives drugs to R using a drug schedule on
- N performs daily health checks pressure and enters it to the MobileTApp
- D enters R health data to the system
- CO enters info about daily activites, menus etc.

## TODO
- [x] Integrate my Smart Watch to Google Fit
- [x] Get access to GA (via Browser)
- [x] Get data from GA using .NET Code
- [x] Setup Azure Functions manipulating dummy daya
- [x] Add documentation diagram that shows projects (Mermaid)
- [x] Store random data in No-SQL storage in Azure
- [ ] Setup ServiceBus that will be integrated with connectors
- [ ] Setup Azure Functions for quering data From GA (e.g. 1 per day)
- [ ] Store health data in No-SQL db in Azure
- [ ] Prepare a C# project with domain concerns (reusable)
- [ ] Prepare FE in React for FM
- [ ] Prepare MTApp in .net MAUI
- [ ] Prepare backend in C# (minimal API or Azure Function)


## Links
1. [Markdown Cheat Sheet](https://www.markdownguide.org/cheat-sheet/)
1. [C# Corner - Getting Started](https://www.c-sharpcorner.com/article/getting-started-with-google-fitness-rest-api-part-2/)
1. [Google Api Playground](https://developers.google.com/oauthplayground/)
1. [Example of C# code using FitnessApi](https://keestalkstech.com/2016/07/getting-your-weight-from-google-fit-with-c/)
1. [Official Google FitnessApi Doc](https://developers.google.com/api-client-library/dotnet/apis/fitness/v1)
1. [Marmaid Docks](https://mermaid.js.org/syntax/flowchart.html)


## Credential
Get OAuth Token json configuration from Google

## Miscellaneous
[Azure Free Servies](https://azure.microsoft.com/en-us/pricing/free-services)
