## Live demo: https://wonderful-meadow-00852321e.4.azurestaticapps.net/

## About project
## Application fetches captions from a YouTube video and allows to save them as PDF. C# backend and Angular frontend.

## Local setup:
1. Clone the repository
2. In the frontend project in summary-service.ts set url to local backend one
3. In the backend project right click on WebApi and choose "Manage User Secrets" and add these two:
`{
  "Groq": {
    "ApiKey": "" // you need to have your Groq API secret
  },
  "TranscriptApi": {
    "ApiKey": "" // you need to have your supadata.ai API secret
  }
}`
Above keys are stored safely in Azure for the live demo purposes.
4. When backend and frontend are running together, test the app.
