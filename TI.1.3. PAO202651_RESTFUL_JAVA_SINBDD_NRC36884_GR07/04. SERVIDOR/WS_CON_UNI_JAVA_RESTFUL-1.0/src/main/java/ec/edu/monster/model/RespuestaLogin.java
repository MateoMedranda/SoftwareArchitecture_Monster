package ec.edu.monster.model;

import java.io.Serializable;

public class RespuestaLogin implements Serializable {

    private String token;
    private long expiraEn;

    public RespuestaLogin() {}

    public RespuestaLogin(String token, long expiraEn) {
        this.token = token;
        this.expiraEn = expiraEn;
    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public long getExpiraEn() {
        return expiraEn;
    }

    public void setExpiraEn(long expiraEn) {
        this.expiraEn = expiraEn;
    }
}