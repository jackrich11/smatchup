import { useState } from "react";
import axios from "axios";
import { Outlet, useLoaderData } from "react-router-dom";
import Header from "./components/Header";

export const getEnvVar = function(envVar) {    
    if(process.env.BUILD_ENV === undefined) {
        return import.meta.env["VITE_" + envVar]
    } else if(process.env.BUILD_ENV === "dev") {
        envVar = "DEV_" + envVar
    } else if(process.env.BUILD_ENV === "prod") {
        envVar = "PROD_" + envVar
    } else {
        console.log("ERROR: Couldn't read BUILD_ENV environment variable.")
        return null
    }
    return process.env[envVar]
}

export async function loader() {
    try {
        console.log("ENVS")
        console.log(import.meta.env)
        console.log(process.env)

        let res = await axios.get(getEnvVar("BASE_URL") + "/api/user", { withCredentials: true })

        console.log("RootLoader: " + res.status)
        console.log(res.data)
    
        return res.data
    } catch(error) {
        console.log(error.message)
        return null
    }
}

export default function User() {
    const userData = useLoaderData()
    const [isLoggedIn, setIsLoggedIn] = useState(userData ? true : false)
    const [currUser, setCurrUser] = useState(userData || {})

    return (
        <>
        <div className="header">
            <Header 
                currUser={currUser}
                isLoggedIn={isLoggedIn}
            />
        </div>
        {isLoggedIn
        ? <Outlet context={currUser} />
        : <h1>Login with your Discord account to see what matches are available!</h1>}
        </>
    )
}