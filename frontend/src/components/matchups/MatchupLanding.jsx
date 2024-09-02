import { useNavigate, Link, useOutletContext } from "react-router-dom"
import { Box } from "@mui/material"
import axios from "axios"
import { useEffect, useState } from "react"
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"
import { getEnvVar } from "../../Root"


const getMatchups = async (setMatchups) => {
    try {
        let res = await axios.get(getEnvVar("BASE_URL") + "/api/matchups", { withCredentials: true })

        console.log("Getting matchups: " + res.status)
        console.log(res.data)
        
        setMatchups(res.data)

        return res.data
    } catch(error) {
        console.log(error.message)
        setMatchups([])
        return null
    }
}

export default function MatchupLanding() {
    const nav = useNavigate()
    // const { currUser, connection } = useOutletContext()
    const { currUser } = useOutletContext()
    const [matchups, setMatchups] = useState([]);
    const [connection, setConnection] = useState()

    useEffect(() => {
        console.log("in use effect for matchups")
        getMatchups(setMatchups)
    }, [])

    useEffect(() => {
        const conn = new HubConnectionBuilder()
        .withUrl(getEnvVar("BASE_URL") + "/message")
        .configureLogging(LogLevel.Information)
        .build();

        async function start() {
            try {
                await conn.start();
                console.log("SignalR Connected.");
                setConnection(conn)
                await conn.invoke("SubscribeToMatchups", currUser.Username)
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        conn.onclose(async () => {
            await start();
        });

        conn.on("MatchupsUpdated", () => {
            console.log("Receive MatchupsUpdated Message as " + currUser.Username)
            getMatchups(setMatchups)
        })

        start();
    }, [])


    const create = async() => {
        await unsubscribe()
        nav('create')
    }

    const unsubscribe = async () => {
        await connection.invoke("UnsubscribeFromMatchups", currUser.Username)
    }

    return (
    <>
    <Box sx={{display: 'flex', flexDirection: 'column'}}>
        {matchups && matchups.map((m) => (!m.visitor && m.creator != currUser.Username && <Link onClick={unsubscribe} to={m.matchupId} key={m.creator} state={m}>{m.creator}</Link>))}
        <button onClick={create}>Create Matchup!</button>
    </Box>
    </>
    )
}