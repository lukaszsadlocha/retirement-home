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
- [ ] Get data from GA using .NET Code
- [ ] Store health data in No-SQL db
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

## Credential
Google Api Key is on Slack!

## Misc
- GoogleApi Playground - get all datasets
Request URI https://fitness.googleapis.com/fitness/v1/users/me/dataSources
- Google Api Playground - get height https://fitness.googleapis.com/fitness/v1/users/me/dataSources/raw:com.google.height:com.google.android.apps.fitness:user_input/dataPointChanges

---

# How to set it up for a rin

1. Creare a file `client_secrets.json` in the project folder with Google OAuth Token (you can get it from: https://console.cloud.google.com/apis/credentials
1. Mark it property in Visual Studio as "Copy Always"
1. Run the app
