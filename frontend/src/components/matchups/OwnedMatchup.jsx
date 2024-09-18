import { Box, Typography } from "@mui/material";

export default function OwnedMatchup({ matchup, visitor }) {

    return (
        <>
        <Box>
            {matchup.isCreator && !visitor && 
            <Typography>Waiting for someone to join the matchup...</Typography>
            }
            {visitor && 
            <Typography>{visitor} has joined the matchup!</Typography>}
        </Box>
        </>
    )
}