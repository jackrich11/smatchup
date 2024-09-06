import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Box } from '@mui/material';
import * as React from 'react';
import { Link, Outlet, useLoaderData, useOutletContext } from 'react-router-dom';

export default function Matchups() {
    const currUser = useOutletContext()

    console.log(currUser)

    return (<>
    <Box sx={{
        display: "flex",
        flexDirection: "column",
        border: 3,
        borderRadius: "13px",
        borderColor: "orange",
        padding: "3rem",
    }}>
        <h1>Matches page for {currUser.Username}</h1>
        <Outlet context={{ currUser }} />
    </Box>
    </>)
}