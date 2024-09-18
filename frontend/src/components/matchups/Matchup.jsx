import { useLocation, useNavigate, useOutlet, useOutletContext } from "react-router-dom";
import axios from "axios";
import { Button, Stack } from "@mui/material";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useEffect, useState } from "react";
import OwnedMatchup from "./OwnedMatchup";
import VisitingMatchup from "./VisitingMatchup";
import { getEnvVar } from "../../utils";

async function closeMatchup(matchupId) {
    try {
        await axios.delete(getEnvVar("BASE_URL") + "/api/matchups/" + matchupId, { withCredentials: true })
        return true
    } catch(error) {
        return false
    }
}

export default function Matchup() {
    const nav = useNavigate()
    const { currUser } = useOutletContext()
    const [connection, setConnection] = useState()
    const matchup = useLocation().state
    const [visitor, setVisitor] = useState(null)
    const [joined, setJoined] = useState(false)

    useEffect(() => {
        const addVisitorToMatchup = async (visitor) => {
            let config = { withCredentials: true }

            try {
                var res = await axios.put(getEnvVar("BASE_URL") + "/api/matchups/" + matchup.matchupId + "/" + visitor, {}, config)
                console.log("Added visitor to matchup: " + res.data)
            } catch(error) {
                console.log("Error: " + error.message)
            }
        }

        const removeVisitorFromMatchup = async () => {
            let config = { withCredentials: true }

            try {
                var res = await axios.put(getEnvVar("BASE_URL") + "/api/matchups/" + matchup.matchupId, {}, config)
                console.log("Removed visitor from matchup: " + res.data)
            } catch(error) {
                console.log("Error: " + error.message)
            }
        }

        const invokeAddToMatchupGroup = async () => {
            await conn.invoke("AddToMatchupGroup", matchup.matchupId, currUser.Username)
            console.log("Added user to matchup.")
        }

        const conn = new HubConnectionBuilder()
        .withUrl(getEnvVar("BASE_URL") + "/message")
        .configureLogging(LogLevel.Information)
        .build();

        async function start() {
            try {
                await conn.start();
                console.log("SignalR Connected.");
                setConnection(conn)

                console.log("Invoking add to matchup")
                invokeAddToMatchupGroup()

                if(!matchup.isCreator) {
                    try {
                        console.log("Joining matchup")
                        await conn.invoke("JoinMatchup", matchup.matchupId, currUser.Username);
                        setJoined(true)
                    } catch (err) {
                        console.error(err);
                        setJoined(false)
                    }
                }
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        conn.onclose(async () => {
            await start();
        });

        conn.on("MatchupJoined", async (visitor) => {
            if(matchup.isCreator) {
                console.log("SOMEONE JOINED THE MATCHUP: " + visitor)
                addVisitorToMatchup(visitor)
                setVisitor(visitor)
            } else {
                console.log("YOU JOINED THE MATCHUP")
            }
        });

        conn.on("ExitMatchup", (username) => {
            if(matchup.isCreator) {
                //In YOUR OWN matchup
                if(username === currUser.Username) {
                    console.log("Deleting matchup.")
                    let res = closeMatchup(matchup.matchupId)

                    if(res) {
                        console.log("Successfully closed matchup.")
                    } else {
                        console.log("Failed to close matchup.")
                    }
                    nav("..", { relative: "path"})
                } else {
                    removeVisitorFromMatchup(visitor)
                    setVisitor(null)
                }
            } else {
                //Visiting OTHER USER's matchup
                console.log("Visiting matchup, got exit request from " + currUser.Username)
                if(username !== currUser.Username) {
                    alert("Owner has closed the matchup.")
                }
                nav("..", { relative: "path" })
            }
        })

        start()
    }, [])

    const exit = async () => {
        await connection.invoke("RemoveFromMatchupGroup", matchup.matchupId, currUser.Username)
    }

    const joinMatchup = async () => {
        try {
            console.log("Joining matchup")
            await connection.invoke("JoinMatchup", matchup.matchupId, currUser.Username);
        } catch (err) {
            console.error(err);
        }
    }
  
    return (
    <>
    <Stack spacing={3}>
        {matchup.isCreator ?
        <OwnedMatchup matchup={matchup} visitor={visitor}/> :
        <VisitingMatchup matchup={matchup} joined={joined}/>}
        <Button variant="outlined" onClick={exit}>Exit</Button>
    </Stack>
    </>
    );
  }