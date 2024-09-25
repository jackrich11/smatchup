import { Box, Grid, Typography } from "@mui/material";

export default function OwnedMatchup({ matchup, visitor }) {

    return (
        <>
        {/* <Grid container alignContent={"center"} spacing={1}> */}
            {matchup.isCreator && !visitor && 
            <Typography>Waiting for someone to join the matchup...</Typography>
            }
            {visitor && 
            <Typography>{visitor} has joined the matchup!</Typography>}
        {/* </Grid> */}
        </>
    )
}