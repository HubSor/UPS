
export const InfoRow = ({ name, value }: { name: string, value?: string }) => {
    return <div className="form-group row justify-content-center">
        <label className="col-sm-6 col-form-label info-label">{name}</label>
        <div className="col-6">
            <input type="text" readOnly className="form-control-plaintext info-value" value={value ?? ""} />
        </div>
    </div>
}