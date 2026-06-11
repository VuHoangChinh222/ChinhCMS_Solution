/* 
 * HEROBANNER COMPONENT - REUSABLE HIGH-END HERO COMPONENT
 * Sinh viên: Vũ Hoàng Chính
 * Môn học: Chuyên đề ASP.NET Core & ReactJS
 */

import React from 'react';
import '../assets/css/HeroBanner.css';

const HeroBanner = ({ tag, title, desc, image, buttonText, onButtonClick }) => {
  return (
    <div className="hero-banner-container" style={{ backgroundImage: `url(${image})` }}>
      <div className="hero-banner-overlay"></div>
      <div className="hero-banner-content">
        {tag && <span className="hero-banner-tag">{tag}</span>}
        {title && <h1 className="hero-banner-title">{title}</h1>}
        {desc && <p className="hero-banner-desc">{desc}</p>}
        {buttonText && (
          <button className="btn btn-primary hero-banner-btn" onClick={onButtonClick}>
            {buttonText}
          </button>
        )}
      </div>
    </div>
  );
};

export default HeroBanner;
