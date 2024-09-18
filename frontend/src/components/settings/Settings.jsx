import { Stack, Typography } from "@mui/material";

export default function Settings({ settings }) {
    return (
    <>
    <Stack spacing={1}>
        <Typography>Region: {settings.region}</Typography>
        <Typography>Skill Level: {settings.playerSkillLevel}</Typography>
        <Typography>Has Delay Mod: {settings.hasDelayMod ? "Yes" : "No"}</Typography>
        </Stack>
    </>
    )
}