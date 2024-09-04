import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import Root from './Root.jsx'
import { loader as rootLoader } from './Root.jsx'
import MatchupForm, { action as createMatchupAction } from './components/matchups/MatchupForm.jsx'
import MatchupLanding from './components/matchups/MatchupLanding.jsx'
// import { loader as matchupLoader } from './components/Matchup.jsx'
import Matchups from './views/Matchups.jsx'
import Stats from './views/Stats.jsx'
import Settings from './views/Settings.jsx'
import Matchup from './components/matchups/Matchup.jsx'
import UpdateSettingsForm from './components/settings/UpdateSettingsForm.jsx'
import CurrentSettings from './components/settings/CurrentSettings.jsx'
import { getEnvVar } from './utils.js'

const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    loader: rootLoader,
    children: [
      {
        path: "/",
        element: <h1>Select a menu option or click on the Smatchup logo to see available matches!</h1>
      },
      {
        path: "matches",
        element: <Matchups />,
        children: [
          {
            path: "",
            element: <MatchupLanding />,
          },
          {
            path: "create",
            element: <MatchupForm />,
          },
          {
            path: ":matchupId",
            element: <Matchup />,
          }
        ]
      },
      {
        path: "stats",
        element: <Stats />,
        loader: () => {
          console.log("in stats loader")
          return {}
        }
      },
      {
        path: "settings",
        element: <Settings />,
        children: [
          {
            path: "",
            element: <CurrentSettings/>
          },
          {
            path: "update",
            element: <UpdateSettingsForm/>
          }
        ]
      },
      
    ]
  }
]);

ReactDOM.createRoot(document.getElementById('root')).render(
  // <React.StrictMode>
    <RouterProvider router={router} />
  // </React.StrictMode>,
)
