import { Grid } from '@mui/material';
import * as React from 'react';
import { Outlet, useOutletContext } from 'react-router-dom';

export default function Matchups() {
    const currUser = useOutletContext()

    console.log(currUser)

    return (<>
    <Grid container justifyContent={"center"}>
        <Outlet context={{ currUser }} />
    </Grid>
    </>)
}