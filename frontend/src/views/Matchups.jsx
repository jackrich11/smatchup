import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import * as React from 'react';
import { Link, Outlet, useLoaderData, useOutletContext } from 'react-router-dom';

export default function Matchups() {
    const currUser = useOutletContext()

    console.log(currUser)

    return (<>
    <h1>Matches page for {currUser.Username}</h1>
    <Outlet context={{ currUser }} />
    </>)
}