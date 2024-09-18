import * as React from 'react';
import { Outlet, useOutletContext } from 'react-router-dom';

export default function Settings() {
    const currUser = useOutletContext()

    return (<>
    <Outlet context={currUser}/>
    </>)
}