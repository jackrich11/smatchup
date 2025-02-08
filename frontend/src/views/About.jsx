import { Stack, Typography } from "@mui/material";

export default function About() {

    return(
        <>
        <Stack spacing={1}>
            <Typography variant="h6">About Smatchup</Typography>
            <Typography>Smatchup was created as a place where you can find others to play Smash Ultimate with, without worrying about ranked ladders, ELO, or stressing out. Use it as a tool to find specific matchup practcice or just someone to play some matches with. Head to the 'Matches' tab to find a matchup or create your own!</Typography>
        </Stack>
        </>
    )
}