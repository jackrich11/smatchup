import { Grid } from '@mui/material';
import * as React from 'react';
import { Outlet, useOutletContext } from 'react-router-dom';

export default function Settings() {
    const currUser = useOutletContext()

    return (<>
    <Grid container justifyContent={"center"}>
        <Outlet context={currUser}/>
    </Grid>
    </>)
}