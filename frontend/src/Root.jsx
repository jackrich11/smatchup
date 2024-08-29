import { useState } from "react";
import axios from "axios";
import { Outlet, useLoaderData } from "react-router-dom";
import Header from "./components/Header";

export async function loader() {
    try {
        let res = await axios.get(import.meta.env.VITE_BASE_URL + "/api/user", { withCredentials: true })

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