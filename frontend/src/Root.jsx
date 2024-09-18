import { useState } from "react";
import axios from "axios";
import { Outlet, useLoaderData } from "react-router-dom";
import Header from "./components/Header";
import { getEnvVar } from "./utils";
import { Box, useTheme } from "@mui/material";

export async function loader() {
    try {
        let res = await axios.get(getEnvVar("BASE_URL") + "/api/user", { withCredentials: true })
        return res.data
    } catch(error) {
        console.log(error.message)
        return null
    }
}

export default function User() {
    const userData = useLoaderData()
    const theme = useTheme()
    const [isLoggedIn, setIsLoggedIn] = useState(userData ? true : false)
    const [currUser, setCurrUser] = useState(userData || {})

    return (
        <>
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
            <Box sx={{
                margin: '5rem'
            }}>
                <Header 
                    currUser={currUser}
                    isLoggedIn={isLoggedIn}
                />
            </Box>
            <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                border: 1,
                borderRadius: '13px',
                borderColor: 'gray',
                padding: '3rem',
                width: '30rem'
            }}>
                {isLoggedIn
                ? <Outlet context={currUser} />
                : <h1>Login with your Discord account to see what matches are available!</h1>}
            </Box>
        </Box>
        </>
    )
}