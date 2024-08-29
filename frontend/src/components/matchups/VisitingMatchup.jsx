export default function VisitingMatchup({ matchup, joined }) {

    return (
        <>
        {joined ?
        <p>In matchup with {matchup && matchup.creator}</p> :
        <p>Joining matchup...</p>}
        </>
    )
}