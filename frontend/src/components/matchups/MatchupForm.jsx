import { Autocomplete, TextField } from "@mui/material";
import Characters from "../../types/Characters";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import axios from "axios";
import { getEnvVar } from "../../utils";

export async function action(data) {
    if(!data.canPlay.length || !data.lookingFor.length) {
        return {
            success: false,
            message: "Error: Must have at least one character selected for both options."
        }
    }

    let config = {
        headers: {
            'Content-Type': 'application/json'
        },
        withCredentials: true
    }
    try {
        var res = await axios.post(getEnvVar("BASE_URL") + "/api/matchups", data, config)
        console.log("Created matchup: " + res.data)
        return { success: true, matchup: res.data}
    } catch(error) {
        console.log("Error: " + error.message)
        return { success: false, message: "Error: Failed to create matchup. Please make sure you have not already created a matchup." }
    }
}

export default function MatchupForm() {
    const nav = useNavigate()
    const [lookingFor, setLookingFor] = useState([])
    const [canPlay, setCanPlay] = useState([])
    const [matchupCreated, setMatchupCreated] = useState(null)

    const close = () => nav("..", { relative: "path" })

    const createMatchup = async () => {
        var res = await action({ lookingFor, canPlay })
        setMatchupCreated(res)

        if(res.success) {
            const matchup = {isCreator: true, ...res.matchup}
            console.log("RES MATCHUP ID: " + res.matchup.creator)
            // nav("../" + res.matchup.matchupId, { relative: "path", state: { matchup: res.matchup, isCreator: true } })
            nav("../" + res.matchup.matchupId, { relative: "path", state: matchup })
        }
    }

    return (<>
    <div className="matchup-form" >
        <Autocomplete
        name="lookingFor"
        multiple
        limitTags={2}
        id="multiple-limit-tags"
        options={Characters}
        getOptionLabel={(option) => option.name}
        onChange={(event, newValue) => {
            console.log(newValue)
            setLookingFor(newValue)
            console.log(lookingFor)
        }}
        renderInput={(params) => (
            <TextField {...params} label="Characters you are looking for." placeholder="Looking for..." />
        )}
        sx={{ width: '500px' }}
        />
        <Autocomplete
        name="canPlay"
        multiple
        limitTags={2}
        id="multiple-limit-tags"
        options={Characters}
        getOptionLabel={(option) => option.name}
        onChange={(event, newValue) => {
            setCanPlay(newValue)
        }}
        renderInput={(params) => (
            <TextField {...params} label="Characters you can play." placeholder="Can play..." />
        )}
        sx={{ width: '500px' }}
        />
        {lookingFor.map(l => <TextField style={{display: "none"}} key={l.name} name={"LookingFor" + l.name} value={JSON.stringify(l)}></TextField>)}
        {canPlay.map(l => <TextField style={{display: "none"}} key={l.name} name={"CanPlay" + l.name} value={JSON.stringify(l)}></TextField>)}

        <button onClick={createMatchup}>Create</button>
        {matchupCreated && 
        <p style={{ color: matchupCreated.success ? "green" : "red"}}>{matchupCreated.success ? "Successfully created matchup!" : matchupCreated.message}</p>
        }
    </div>
    <button onClick={close}>Close</button>
    </>)
}