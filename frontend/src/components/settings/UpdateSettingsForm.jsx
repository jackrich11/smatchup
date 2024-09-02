//TODO: Create page to show current settings and then form to set/update user settings

import { useEffect, useState } from "react"
import { regions }from "../../types/Regions"
import { skillLevels } from "../../types/SkillLevels"
import { Button, Checkbox, FormControl, FormControlLabel, InputLabel, MenuItem, Select } from "@mui/material"
import { useLocation, useNavigate, useOutletContext } from "react-router-dom"
import axios from "axios"
import { getEnvVar } from "../../Root"

export default function UpdateSettingsForm() {
    const nav = useNavigate()
    const currUser = useOutletContext()
    const settings = useLocation().state
    const [region, setRegion] = useState(settings ? settings.region : regions[regions.length - 1])
    const [skillLevel, setSkillLevel] = useState(settings ? settings.playerSkillLevel : skillLevels[skillLevels.length - 1])
    const [hasDelayMod, setHasDelayMod] = useState(settings ? settings.hasDelayMod : false)

    const close = () => nav("..", { path: "relative"})

    const submit = async () => {

        let data = {
            username: currUser.Username,
            region,
            skillLevel,
            hasDelayMod
        }
        console.log(data)

        let config = {
            headers: {
                'Content-Type': 'application/json'
            },
            withCredentials: true
        }
        try {
            var res = await axios.post(getEnvVar("BASE_URL") + "/api/settings", data, config)
            close()
            return true
        } catch(error) {
            console.log("Error: " + error.message)
            return false
        }
    }

    return (
    <>
    <FormControl fullWidth>
    <InputLabel id="demo-simple-select-label">Region</InputLabel>
    <Select
        labelId="demo-simple-select-label"
        id="demo-simple-select"
        value={region}
        label="Region"
        onChange={(e) => setRegion(e.target.value)}
    >
        {regions.map((r) => 
        <MenuItem key={r} value={r}>{r}</MenuItem>)}
    </Select>
    </FormControl>
    <FormControl fullWidth>
    <InputLabel id="demo-simple-select-label">Skill Level</InputLabel>
    <Select
        labelId="demo-simple-select-label"
        id="demo-simple-select"
        value={skillLevel}
        label="Skill Level"
        onChange={(e) => setSkillLevel(e.target.value)}
    >
        {skillLevels.map((s) => 
        <MenuItem key={s} value={s}>{s}</MenuItem>)}
    </Select>
    </FormControl>
    <FormControlLabel
        value="Delay Mod"
        control={<Checkbox
        checked={hasDelayMod}
        onChange={(e) => setHasDelayMod(e.target.checked)}
        inputProps={{ 'aria-label': 'controlled' }}
        />}
        label="Has Delay Mod"
        labelPlacement="start"
    />
    <Button onClick={submit}>Submit</Button>
    <Button onClick={close}>Close</Button>
    </>
    )
}