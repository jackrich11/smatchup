import { useEffect, useState } from "react"
import { withCookies } from "react-cookie"
import { useNavigate, useOutletContext } from "react-router-dom"
import axios from "axios"
import { Button } from "@mui/material"
import Settings from "./Settings"
import { getEnvVar } from "../../Root.jsx"

export default function CurrentSettings() {
    const nav = useNavigate()
    const currUser = useOutletContext()
    const [settings, setSettings] = useState()

    useEffect(() => {
        const getSettings = async () => {
            try {
                let res = await axios.get(getEnvVar("BASE_URL") + "/api/settings/" + currUser.Username, { withCredentials: true })
                setSettings(res.data)
                console.log(res)
                console.log("got user settings")
            } catch(e) {
                console.log(e.message)
            }
        }
        getSettings()
    }, [])

    return (
        <>
        {settings ?
        <Settings settings={settings}/> :
        <p>No settings for {currUser.Username}</p>}
        <Button onClick={() => nav("update", { state: settings })}>{settings ? "Update Settings" : "Create your settings!"}</Button>
        </>
    )
}