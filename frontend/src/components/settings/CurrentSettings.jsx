import { useEffect, useState } from "react"
import { withCookies } from "react-cookie"
import { useNavigate, useOutletContext } from "react-router-dom"
import axios from "axios"
import { Button, Stack, Typography } from "@mui/material"
import Settings from "./Settings"
import { getEnvVar } from "../../utils"

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
        <Stack spacing={3} justifyContent={"center"}>
            {settings ?
            <Stack spacing={1}>
                <Typography variant="h6">Settings for {currUser.Username}</Typography>
                <Settings settings={settings}/>
            </Stack> :
            <Typography>No settings for {currUser.Username}</Typography>}
            <Button variant="outlined" onClick={() => nav("update", { state: settings })}>{settings ? "Update Settings" : "Create your settings!"}</Button>
        </Stack>
        </>
    )
}