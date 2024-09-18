import { Box, Typography } from "@mui/material";

export default function VisitingMatchup({ matchup, joined }) {

    return (
        <>
        <Box>
            {joined ?
            <Typography>In matchup with {matchup && matchup.creator}</Typography> :
            <Typography>Joining matchup...</Typography>}
        </Box>
        </>
    )
}