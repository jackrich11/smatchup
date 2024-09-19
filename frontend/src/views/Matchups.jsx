import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Box, Grid } from '@mui/material';
import * as React from 'react';
import { Link, Outlet, useLoaderData, useOutletContext } from 'react-router-dom';

export default function Matchups() {
    const currUser = useOutletContext()

    console.log(currUser)

    return (<>
    {/* <Box sx={{
        display: "flex",
        flexDirection: "column",
        paddingX: "3rem"
    }}>
        <Outlet context={{ currUser }} />
    </Box> */}
    <Grid container>
        <Outlet context={{ currUser }} />
    </Grid>
    </>)
}