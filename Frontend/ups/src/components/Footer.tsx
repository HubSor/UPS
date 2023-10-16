export default function Footer(){
    return <div className="footer">
        <div className="container">
            <span className="text-muted">
                Uniwersalna Platforma Sprzedażowa {process.env.REACT_APP_VERSION}
            </span>
        </div>
    </div>
}