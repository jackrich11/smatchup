import * as React from 'react';
import { Outlet, useOutletContext } from 'react-router-dom';

export default function Settings() {
    const currUser = useOutletContext()

    return (<>
    <h1>Settings page</h1>
    <Outlet context={currUser}/>
    </>)
}