import { useState } from "react";
import axios from "axios";
import { Outlet, useLoaderData } from "react-router-dom";
import Header from "./components/Header";
import { getEnvVar } from "./utils";
import { Box, Grid, Typography, useTheme } from "@mui/material";

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
        <Grid container sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyItems: 'center'}}>
            <Grid sx={{ margin: '5rem' }}>
                <Header 
                    currUser={currUser}
                    isLoggedIn={isLoggedIn}
                />
            </Grid>
                {isLoggedIn
                ? <Grid sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyItems: 'center',
                    border: 1,
                    borderRadius: '13px',
                    borderColor: 'gray',
                    padding: '3rem',
                    minWidth: '30rem',
                    minHeight: '15rem'
                  }}>
                    <Outlet context={currUser} />
                  </Grid>
                : <Grid spacing={12}>
                    <Typography>Login with your Discord account to see what matches are available!</Typography>
                  </ Grid>}
            
        </Grid>
        </>
    )
}